namespace Carter.App.Route.Sessions
{
    using System;
    using System.IO;
    using System.Linq;

    using Carter;

    using Carter.App.Lib.Authentication;
    using Carter.App.Lib.Generate;
    using Carter.App.Lib.Mongo;
    using Carter.App.Lib.Network;

    using Carter.App.Route.PreSecurity;

    using Carter.Request;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;

    public class Sessions : CarterModule
    {
        public Sessions(IMongoDatabase db)
            : base("/sessions")
        {
            this.Before += PreSecurity.GetSecurityCheck(db);

            this.Post("/", async (req, res) =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");

                string uniqueID = Generate.GetRandomId(4);
                var filter = Builders<BsonDocument>.Filter.Eq("id", uniqueID);

                // Will loop until a unique id is found
                // Needed because the ids that are generated are from a small number of combinations
                while ((await sessions.Find(filter).FirstOrDefaultAsync()) != null)
                {
                    uniqueID = Generate.GetRandomId(4);
                    filter = Builders<BsonDocument>.Filter.Eq("id", uniqueID);
                }

                var accessTokens = db.GetCollection<BsonDocument>("users.tokens.access");
                string token = req.Cookies["ExperienceCapture-Access-Token"]; // Has to exist due to PreSecurity Check

                var accessTokenDoc = await accessTokens.FindEqAsync("hash", PasswordHasher.Hash(token));

                var users = db.GetCollection<BsonDocument>("users");

                var projection = Builders<BsonDocument>.Projection.Exclude("_id");
                var filterUser = Builders<BsonDocument>.Filter.Eq("_id", accessTokenDoc["user"].AsObjectId);

                var user = await users
                    .Find(filterUser)
                    .Project(projection)
                    .FirstOrDefaultAsync();

                var sessionDoc = new
                {
                    isOpen = true,
                    isExported = false,
                    isPending = false,
                    user = user, // Copying user data instead of referencing so it can never change with the session
                    createdAt = new BsonDateTime(DateTime.Now),
                    tags = new BsonArray(),
                };

                var bsonDoc = sessionDoc.ToBsonDocument();
                bsonDoc.Add("id", uniqueID); // Added afterwards because the library will try to use it as the primary key

                await sessions.InsertOneAsync(bsonDoc);
                bsonDoc.Remove("_id");
                bsonDoc.Add("isOngoing", true); // isOngoing is a proxy variable and will always start out as true

                var sessionCollection = db.GetCollection<BsonDocument>($"sessions.{uniqueID}");

                // Secondary index or else Mongo will fail on large queries
                // It has a limit for max number of documents on properties
                // Without an index
                var index = Builders<BsonDocument>.IndexKeys;
                var key = index.Ascending("frameInfo.realtimeSinceStartup");
                var options = new CreateIndexOptions();

                var model = new CreateIndexModel<BsonDocument>(key, options);
                await sessionCollection.Indexes.CreateOneAsync(model);

                string json = JsonQuery.FulfilEncoding(req.Query, bsonDoc);
                if (json != null)
                {
                    JsonResponce.FromString(res, json);
                    return;
                }

                BsonResponse.FromDoc(res, bsonDoc);
            });

            this.Get("/", async (req, res) =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");
                var projection = Builders<BsonDocument>.Projection.Exclude("_id");

                var builder = Builders<BsonDocument>.Filter;
                FilterDefinition<BsonDocument> filter = builder.Empty;

                var range = new BsonDateTime(DateTime.Now.AddSeconds(-120)); // Two minutes

                // TODO: add a way to query based on tag

                // Three potential options: null, true, or false
                if (req.Query.As<bool?>("isOngoing") != null)
                {
                    bool isOngoing = req.Query.As<bool>("isOngoing");
                    filter &= builder.Eq("isOpen", isOngoing);

                    if (isOngoing)
                    {
                        filter &= (builder.Exists("lastCaptureAt", false)
                            & builder.Gt("createdAt", range))
                            | builder.Gt("lastCaptureAt", range);
                    }
                    else
                    {
                        filter |= (builder.Exists("lastCaptureAt", false)
                            & builder.Lt("createdAt", range))
                            | builder.Lt("lastCaptureAt", range);
                    }
                }

                var sorter = Builders<BsonDocument>.Sort.Descending("createdAt");
                var sessionDocs = await sessions
                    .Find(filter)
                    .Project(projection)
                    .Sort(sorter)
                    .ToListAsync();

                var sessionsDocsWithOngoing = sessionDocs.Select((s) =>
                {
                    bool isStarted = false;
                    if (s.GetValue("lastCaptureAt", null) != null)
                    {
                        isStarted = true;
                    }

                    bool isOngoing;
                    if (s["isOpen"].AsBoolean)
                    {
                        isOngoing = range.CompareTo(s["createdAt"]) < 0
                            || (isStarted
                            && range.CompareTo(s["lastCaptureAt"]) < 0);
                    }
                    else
                    {
                        isOngoing = range.CompareTo(s["createdAt"]) > 0
                            && (isStarted
                            && range.CompareTo(s["lastCaptureAt"]) > 0);
                    }

                    s.Add("isOngoing", isOngoing);

                    return s;
                });

                var clientValues = new
                {
                    // Bson documents can't start with an array like Json, so a wrapping object is used instead
                    contentArray = sessionsDocsWithOngoing,
                };
                var clientDoc = clientValues.ToBsonDocument();

                string json = JsonQuery.FulfilEncoding(req.Query, clientDoc);
                if (json != null)
                {
                    JsonResponce.FromString(res, json);
                    return;
                }

                BsonResponse.FromDoc(res, clientDoc.ToBsonDocument());
            });

            this.Post("/{id}", async (req, res) =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");

                string uniqueID = req.RouteValues.As<string>("id");
                var filter = Builders<BsonDocument>.Filter.Eq("id", uniqueID);
                var sessionDoc = await sessions.Find(filter).FirstOrDefaultAsync();

                if (sessionDoc == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                if (!sessionDoc["isOpen"].AsBoolean)
                {
                    res.StatusCode = 400;
                    return;
                }

                BsonDocument document;
                if (JsonQuery.CheckDecoding(req.Query))
                {
                    using (var ms = new MemoryStream())
                    {
                        await req.Body.CopyToAsync(ms);
                        ms.Position = 0;
                        document = BsonSerializer.Deserialize<BsonDocument>(ms);
                    }
                }
                else
                {
                    string json = await req.Body.AsStringAsync();
                    document = BsonDocument.Parse(json);
                }

                var sessionCollection = db.GetCollection<BsonDocument>($"sessions.{uniqueID}");

                // These calls not awaitted for max performance
                // Error propagation is ignored
                _ = sessionCollection.InsertOneAsync(document);

                // This property is undefined on the session document until the first call of this endpoint
                var update = Builders<BsonDocument>.Update
                    .Set("lastCaptureAt", new BsonDateTime(DateTime.Now));

                _ = sessions.UpdateOneAsync(filter, update);

                BasicResponce.Send(res);
            });

            this.Get("/{id}", async (req, res) =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");

                string uniqueID = req.RouteValues.As<string>("id");
                var projection = Builders<BsonDocument>.Projection.Exclude("_id");
                var filter = Builders<BsonDocument>.Filter.Eq("id", uniqueID);

                var sessionDoc = await sessions
                    .Find(filter)
                    .Project(projection)
                    .FirstOrDefaultAsync();

                if (sessionDoc == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                var range = new BsonDateTime(DateTime.Now.AddSeconds(-120)); // Two minutes
                bool isStarted = false;

                if (sessionDoc.GetValue("lastCaptureAt", null) != null)
                {
                    isStarted = true;
                }

                bool isOngoing;
                if (sessionDoc["isOpen"].AsBoolean)
                {
                    isOngoing = range.CompareTo(sessionDoc["createdAt"]) < 0
                        || (isStarted
                        && range.CompareTo(sessionDoc["lastCaptureAt"]) < 0);
                }
                else
                {
                    isOngoing = range.CompareTo(sessionDoc["createdAt"]) > 0
                        && (isStarted
                        && range.CompareTo(sessionDoc["lastCaptureAt"]) > 0);
                }

                sessionDoc.Add("isOngoing", isOngoing);

                string json = JsonQuery.FulfilEncoding(req.Query, sessionDoc);
                if (json != null)
                {
                    JsonResponce.FromString(res, json);
                    return;
                }

                BsonResponse.FromDoc(res, sessionDoc);
            });

            this.Delete("/{id}", async (req, res) =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");

                string uniqueID = req.RouteValues.As<string>("id");
                var filter = Builders<BsonDocument>.Filter.Eq("id", uniqueID);
                var update = Builders<BsonDocument>.Update.Set("isOpen", false);
                var sessionDoc = await sessions.Find(filter).FirstOrDefaultAsync();

                if (sessionDoc == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                await sessions.UpdateOneAsync(filter, update);
                BasicResponce.Send(res);
            });
        }
    }
}