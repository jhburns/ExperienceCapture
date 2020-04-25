namespace Carter.App.Route.Sessions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Carter;

    using Carter.App.Lib.Authentication;
    using Carter.App.Lib.Generate;
    using Carter.App.Lib.Network;

    using Carter.App.Route.PreSecurity;
    using Carter.App.Route.Users;

    using Carter.Request;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;

    public class Sessions : CarterModule
    {
        public Sessions(IMongoDatabase db)
            : base("/sessions")
        {
            this.Before += PreSecurity.GetSecurityCheck(db);

            this.Post("/", async (req, res) =>
            {
                var sessions = db.GetCollection<SessionSchema>("sessions");

                string uniqueID = Generate.GetRandomId(4);
                var filter = Builders<SessionSchema>.Filter.Where(s => s.Id == uniqueID);

                // Will loop until a unique id is found
                // Needed because the ids that are generated are from a small number of combinations
                while ((await sessions.Find(filter).FirstOrDefaultAsync()) != null)
                {
                    uniqueID = Generate.GetRandomId(4);
                    filter = Builders<SessionSchema>.Filter.Where(s => s.Id == uniqueID);
                }

                var accessTokens = db.GetCollection<AccessTokenSchema>("users.tokens.access");
                string token = req.Cookies["ExperienceCapture-Access-Token"]; // Has to exist due to PreSecurity Check

                var accessTokenDoc = await (await accessTokens.FindAsync(
                    Builders<AccessTokenSchema>
                        .Filter
                        .Where(a => a.Hash == PasswordHasher.Hash(token))))
                        .FirstOrDefaultAsync();

                var users = db.GetCollection<PersonSchema>("users");
                var filterUser = Builders<PersonSchema>.Filter.Where(u => u.InternalId == accessTokenDoc.User);

                var user = await (await users
                    .FindAsync(filterUser))
                    .FirstOrDefaultAsync();

                var sessionDoc = new SessionSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Id = uniqueID,
                    IsOpen = true,
                    IsExported = false,
                    IsPending = false,
                    User = user, // Copying user data instead of referencing so it can never change with the session
                    CreatedAt = new BsonDateTime(DateTime.Now),
                    Tags = new List<string>(),
                };

                await sessions.InsertOneAsync(sessionDoc);
                var bsonDoc = sessionDoc.ToBsonDocument();
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

                var startRange = new BsonDateTime(DateTime.Now.AddSeconds(-300)); // 5 minutes
                var closeRange = new BsonDateTime(DateTime.Now.AddSeconds(-5)); // 5 seconds

                // TODO: add a way to query based on tag

                // Three potential options: null, true, or false
                if (req.Query.As<bool?>("isOngoing") != null)
                {
                    bool isOngoing = req.Query.As<bool>("isOngoing");
                    filter &= builder.Eq("isOpen", isOngoing);

                    if (isOngoing)
                    {
                        filter &= (builder.Exists("lastCaptureAt", false)
                            & builder.Gt("createdAt", startRange))
                            | builder.Gt("lastCaptureAt", closeRange);
                    }
                    else
                    {
                        filter |= (builder.Exists("lastCaptureAt", false)
                            & builder.Lt("createdAt", startRange))
                            | builder.Lt("lastCaptureAt", closeRange);
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
                        isOngoing = (!isStarted
                            && startRange.CompareTo(s["createdAt"]) < 0)
                            || (isStarted
                            && closeRange.CompareTo(s["lastCaptureAt"]) < 0);
                    }
                    else
                    {
                        isOngoing = false;
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

                // Manual validation, because Fluent Validation would remove extra properties
                if (!document.Contains("frameInfo")
                    || document["frameInfo"].BsonType != BsonType.Document
                    || !document["frameInfo"].AsBsonDocument.Contains("realtimeSinceStartup")
                    || document["frameInfo"]["realtimeSinceStartup"].BsonType != BsonType.Double)
                {
                    res.StatusCode = 400;
                    return;
                }

                var sessionCollection = db.GetCollection<BsonDocument>($"sessions.{uniqueID}");

                // These calls not awaited for max performance
                _ = sessionCollection.InsertOneAsync(document);

                // This lastCaptureAt is undefined on the session document until the first call of this endpoint
                // Export flags are reset so the session can be re-exported
                var update = Builders<BsonDocument>.Update
                    .Set("lastCaptureAt", new BsonDateTime(DateTime.Now))
                    .Set("isExported", false)
                    .Set("isPending", false);

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

                var startRange = new BsonDateTime(DateTime.Now.AddSeconds(-300)); // 5 minutes
                var closeRange = new BsonDateTime(DateTime.Now.AddSeconds(-5)); // 5 seconds
                bool isStarted = false;

                // Check if key exists
                if (sessionDoc.GetValue("lastCaptureAt", null) != null)
                {
                    isStarted = true;
                }

                bool isOngoing;
                if (sessionDoc["isOpen"].AsBoolean)
                {
                    isOngoing = (!isStarted
                        && startRange.CompareTo(sessionDoc["createdAt"]) < 0)
                        || (isStarted
                        && closeRange.CompareTo(sessionDoc["lastCaptureAt"]) < 0);
                }
                else
                {
                    isOngoing = false;
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

    public class SessionSchema
    {
        #pragma warning disable SA1516
        [BsonId]
        public BsonObjectId InternalId { get; set; }

        [BsonElement("id")]
        public string Id { get; set; }

        [BsonElement("isOpen")]
        public bool IsOpen { get; set; }

        [BsonElement("IsExported")]
        public bool IsExported { get; set; }

        [BsonElement("isPending")]
        public bool IsPending { get; set; }

        // Copying user data instead of referencing so it can never change with the session
        // Also so that it is easy to include when exporting
        [BsonElement("user")]
        public PersonSchema User { get; set; }

        [BsonElement("createdAt")]
        public BsonDateTime CreatedAt { get; set; }

        [BsonElement("tags")]
        public List<string> Tags { get; set; }
        #pragma warning restore SA151, SA1300
    }
}