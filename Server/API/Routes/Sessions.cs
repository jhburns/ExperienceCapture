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
                var sessions = db.GetCollection<SessionSchema>(SessionSchema.CollectionName);

                string uniqueID = Generate.GetRandomId(4);
                var filter = Builders<SessionSchema>.Filter.Where(s => s.Id == uniqueID);

                // Will loop until a unique id is found
                // Needed because the ids that are generated are from a small number of combinations
                while ((await sessions.Find(filter).FirstOrDefaultAsync()) != null)
                {
                    uniqueID = Generate.GetRandomId(4);
                    filter = Builders<SessionSchema>.Filter.Where(s => s.Id == uniqueID);
                }

                var accessTokens = db.GetCollection<AccessTokenSchema>(AccessTokenSchema.CollectionName);
                string token = req.Cookies["ExperienceCapture-Access-Token"]; // Has to exist due to PreSecurity Check

                var accessTokenDoc = await accessTokens.Find(
                    Builders<AccessTokenSchema>
                        .Filter
                        .Where(a => a.Hash == PasswordHasher.Hash(token)))
                        .FirstOrDefaultAsync();

                var users = db.GetCollection<PersonSchema>(PersonSchema.CollectionName);
                var filterUser = Builders<PersonSchema>.Filter.Where(p => p.InternalId == accessTokenDoc.User);

                var user = await users
                    .Find(filterUser)
                    .FirstOrDefaultAsync();

                var sessionDoc = new SessionSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Id = uniqueID,
                    User = user, // Copying user data instead of referencing so it can never change with the session
                    CreatedAt = new BsonDateTime(DateTime.Now),
                    Tags = new List<string>(),
                };

                await sessions.InsertOneAsync(sessionDoc);

                // isOngoing is a proxy variable and will always start out as true
                sessionDoc.IsOngoing = true;
                sessionDoc.InternalId = null;
                sessionDoc.User.InternalId = null;

                var sessionCollection = db.GetCollection<BsonDocument>($"sessions.{uniqueID}");

                // Secondary index or else Mongo will fail on large queries
                // It has a limit for max number of documents on properties
                // Without an index, see https://docs.mongodb.com/manual/reference/limits/#Sort-Operations
                var index = Builders<BsonDocument>.IndexKeys;
                var key = index.Ascending("frameInfo.realtimeSinceStartup");
                var options = new CreateIndexOptions();

                var model = new CreateIndexModel<BsonDocument>(key, options);
                await sessionCollection.Indexes.CreateOneAsync(model);

                string json = JsonQuery.FulfilEncoding(req.Query, sessionDoc);
                if (json != null)
                {
                    JsonResponce.FromString(res, json);
                    return;
                }

                BsonResponse.ToBson(res, sessionDoc);
            });

            this.Get("/", async (req, res) =>
            {
                var sessions = db.GetCollection<SessionSchema>(SessionSchema.CollectionName);

                var builder = Builders<SessionSchema>.Filter;
                FilterDefinition<SessionSchema> filter = builder.Empty;

                var startRange = new BsonDateTime(DateTime.Now.AddSeconds(-300)); // 5 minutes
                var closeRange = new BsonDateTime(DateTime.Now.AddSeconds(-5)); // 5 seconds

                // TODO: add a way to query based on tag

                // Three potential options: null, true, or false
                if (req.Query.As<bool?>("isOngoing") != null)
                {
                    bool isOngoing = req.Query.As<bool>("isOngoing");
                    filter &= builder.Where(s => s.IsOpen == isOngoing);

                    if (isOngoing)
                    {
                        filter &= (builder.Where(s => s.LastCaptureAt == null)
                            & builder.Where(s => s.CreatedAt > startRange))
                            | builder.Where(s => s.LastCaptureAt > closeRange);
                    }
                    else
                    {
                        filter |= (builder.Where(s => s.LastCaptureAt == null)
                            & builder.Where(s => s.CreatedAt < startRange))
                            | builder.Where(s => s.LastCaptureAt < closeRange);
                    }
                }

                var sorter = Builders<SessionSchema>.Sort.Descending(s => s.CreatedAt);
                var sessionDocs = await sessions
                    .Find(filter)
                    .Sort(sorter)
                    .ToListAsync();

                var sessionsDocsWithOngoing = sessionDocs.Select((s) =>
                {
                    bool isStarted = false;
                    if (s.LastCaptureAt != null)
                    {
                        isStarted = true;
                    }

                    bool isOngoing;
                    if (s.IsOpen)
                    {
                        isOngoing = (!isStarted
                            && startRange.CompareTo(s.CreatedAt) < 0)
                            || (isStarted
                            && closeRange.CompareTo(s.LastCaptureAt) < 0);
                    }
                    else
                    {
                        isOngoing = false;
                    }

                    s.IsOngoing = isOngoing;
                    s.InternalId = null;
                    s.User.InternalId = null;

                    return s;
                });

                var clientValues = new
                {
                    // Bson documents can't start with an array like Json, so a wrapping object is used instead
                    contentArray = sessionsDocsWithOngoing,
                };

                string json = JsonQuery.FulfilEncoding(req.Query, clientValues);
                if (json != null)
                {
                    JsonResponce.FromString(res, json);
                    return;
                }

                BsonResponse.ToBson(res, clientValues);
            });

            this.Post("/{id}", async (req, res) =>
            {
                var sessions = db.GetCollection<SessionSchema>(SessionSchema.CollectionName);

                string uniqueID = req.RouteValues.As<string>("id");
                var filter = Builders<SessionSchema>.Filter.
                    Where(s => s.Id == uniqueID);

                var sessionDoc = await sessions
                    .Find(filter)
                    .FirstOrDefaultAsync();

                if (sessionDoc == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                if (!sessionDoc.IsOpen)
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

                        try
                        {
                            document = BsonSerializer.Deserialize<BsonDocument>(ms);
                        }
                        catch
                        {
                            // TODO: print exception to debug
                            res.StatusCode = 400;
                            return;
                        }
                    }
                }
                else
                {
                    string json = await req.Body.AsStringAsync();

                    try
                    {
                        document = BsonDocument.Parse(json);
                    }
                    catch
                    {
                        // TODO: print exception to debug
                        res.StatusCode = 400;
                        return;
                    }
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
                var update = Builders<SessionSchema>.Update
                    .Set(s => s.LastCaptureAt, new BsonDateTime(DateTime.Now))
                    .Set(s => s.IsExported, false)
                    .Set(s => s.IsPending, false);

                _ = sessions.UpdateOneAsync(filter, update);

                BasicResponce.Send(res);
            });

            this.Get("/{id}", async (req, res) =>
            {
                var sessions = db.GetCollection<SessionSchema>(SessionSchema.CollectionName);

                string uniqueID = req.RouteValues.As<string>("id");
                var filter = Builders<SessionSchema>.Filter.Where(s => s.Id == uniqueID);

                var sessionDoc = await sessions
                    .Find(filter)
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
                if (sessionDoc.LastCaptureAt != null)
                {
                    isStarted = true;
                }

                bool isOngoing;
                if (sessionDoc.IsOpen)
                {
                    isOngoing = (!isStarted
                        && startRange.CompareTo(sessionDoc.CreatedAt) < 0)
                        || (isStarted
                        && closeRange.CompareTo(sessionDoc.LastCaptureAt) < 0);
                }
                else
                {
                    isOngoing = false;
                }

                sessionDoc.IsOngoing = isOngoing;

                string json = JsonQuery.FulfilEncoding(req.Query, sessionDoc);
                if (json != null)
                {
                    JsonResponce.FromString(res, json);
                    return;
                }

                BsonResponse.ToBson(res, sessionDoc);
            });

            this.Delete("/{id}", async (req, res) =>
            {
                string uniqueID = req.RouteValues.As<string>("id");

                var sessions = db.GetCollection<SessionSchema>(SessionSchema.CollectionName);

                var filter = Builders<SessionSchema>.Filter.Where(s => s.Id == uniqueID);
                var sessionDoc = await sessions
                    .Find(filter)
                    .FirstOrDefaultAsync();

                if (sessionDoc == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                var update = Builders<SessionSchema>.Update
                    .Set(s => s.IsOpen, false);

                await sessions.UpdateOneAsync(filter, update);
                BasicResponce.Send(res);
            });
        }
    }

    public class SessionSchema
    {
        #pragma warning disable SA1516
        [BsonIgnore]
        public const string CollectionName = "sessions";

        [BsonIgnoreIfNull]
        [BsonId]
        public BsonObjectId InternalId { get; set; }

        [BsonElement("id")]
        public string Id { get; set; }

        [BsonElement("isOpen")]
        public bool IsOpen { get; set; } = true;

        [BsonElement("isExported")]
        public bool IsExported { get; set; } = false;

        [BsonElement("isPending")]
        public bool IsPending { get; set; } = false;

        // Copying user data instead of referencing so it can never change with the session
        // Also so that it is easy to include when exporting
        [BsonElement("user")]
        public PersonSchema User { get; set; }

        [BsonElement("createdAt")]
        public BsonDateTime CreatedAt { get; set; }

        [BsonElement("tags")]
        public List<string> Tags { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("isOngoing")]
        public bool? IsOngoing { get; set; } = null;

        [BsonElement("lastCaptureAt")]
        public BsonDateTime LastCaptureAt { get; set; } = null;
        #pragma warning restore SA151, SA1300
    }
}