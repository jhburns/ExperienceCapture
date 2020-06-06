namespace Carter.App.Route.Sessions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Carter;

    using Carter.App.Lib.Authentication;
    using Carter.App.Lib.Generate;
    using Carter.App.Lib.Network;
    using Carter.App.Lib.Repository;

    using Carter.App.Route.PreSecurity;
    using Carter.App.Route.Users;

    using Carter.Request;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;

    public class Sessions : CarterModule
    {
        public Sessions(
            IRepository<AccessTokenSchema> accessRepo,
            IRepository<SessionSchema> sessionRepo,
            IRepository<PersonSchema> personRepo,
            IRepository<BsonDocument> captureRepo)
            : base("/sessions")
        {
            this.Before += PreSecurity.GetSecurityCheck(accessRepo);

            this.Post("/", async (req, res) =>
            {
                string uniqueID = Generate.GetRandomId(4);

                // Will loop until a unique id is found
                // Needed because the ids that are generated are from a small number of combinations
                while (await sessionRepo.FindById(uniqueID) != null)
                {
                    uniqueID = Generate.GetRandomId(4);            
                }

                string token = req.Cookies["ExperienceCapture-Access-Token"]; // Has to exist due to PreSecurity Check

                var accessTokenDoc = await accessRepo.FindOne(
                    Builders<AccessTokenSchema>
                        .Filter
                        .Where(a => a.Hash == PasswordHasher.Hash(token)));

                var filterUser = Builders<PersonSchema>.Filter.Where(p => p.InternalId == accessTokenDoc.User);

                var user = await personRepo
                    .FindOne(filterUser);

                var sessionDoc = new SessionSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Id = uniqueID,
                    User = user, // Copying user data instead of referencing so it can never change in the session
                    CreatedAt = new BsonDateTime(DateTime.Now),
                    Tags = new List<string>(),
                };

                await sessionRepo.Add(sessionDoc);

                // isOngoing is a proxy variable and will always start out as true
                sessionDoc.IsOngoing = true;
                sessionDoc.InternalId = null;
                sessionDoc.User.InternalId = null;

                captureRepo.Configure($"sessions.{uniqueID}");

                // Secondary index or else Mongo will fail on large queries
                // It has a limit for max number of documents on properties
                // Without an index, see https://docs.mongodb.com/manual/reference/limits/#Sort-Operations
                var index = Builders<BsonDocument>.IndexKeys;
                var key = index.Ascending("frameInfo.realtimeSinceStartup");

                await captureRepo.Index(key);

                string json = JsonQuery.FulfilEncoding(req.Query, sessionDoc);
                if (json != null)
                {
                    await res.FromJson(json);
                    return;
                }

                await res.FromBson(sessionDoc);
            });

            this.Get("/", async (req, res) =>
            {
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
                var sessionDocs = await sessionRepo
                    .FindAll(filter, sorter);

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
                    await res.FromJson(json);
                    return;
                }

                await res.FromBson(clientValues);
            });

            this.Post("/{id}", async (req, res) =>
            {
                string uniqueID = req.RouteValues.As<string>("id");

                var sessionDoc = await sessionRepo
                    .FindById(uniqueID);

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

                captureRepo.Configure($"sessions.{uniqueID}");
                await captureRepo.Add(document);

                var filter = Builders<SessionSchema>.Filter.Where(s => s.Id == uniqueID);

                // This lastCaptureAt is undefined on the session document until the first call of this endpoint
                // Export flags are reset so the session can be re-exported
                var update = Builders<SessionSchema>.Update
                    .Set(s => s.LastCaptureAt, new BsonDateTime(DateTime.Now))
                    .Set(s => s.ExportState, ExportOptions.NotStarted);

                await sessionRepo.Update(filter, update);

                await res.FromString();
            });

            this.Get("/{id}", async (req, res) =>
            {
                string uniqueID = req.RouteValues.As<string>("id");

                var sessionDoc = await sessionRepo
                    .FindById(uniqueID);

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
                    await res.FromJson(json);
                    return;
                }

                await res.FromBson(sessionDoc);
            });

            this.Delete("/{id}", async (req, res) =>
            {
                string uniqueID = req.RouteValues.As<string>("id");

                var filter = Builders<SessionSchema>.Filter.Where(s => s.Id == uniqueID);
                var sessionDoc = await sessionRepo
                    .FindById(uniqueID);

                if (sessionDoc == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                var update = Builders<SessionSchema>.Update
                    .Set(s => s.IsOpen, false);

                await sessionRepo.Update(filter, update);
                await res.FromString();
            });
        }
    }

    public class SessionSchema
    {
        #pragma warning disable SA1516
        [BsonIgnoreIfNull]
        [BsonId]
        public BsonObjectId InternalId { get; set; }

        [BsonElement("id")]
        public string Id { get; set; }

        [BsonElement("isOpen")]
        public bool IsOpen { get; set; } = true;

        [BsonElement("exportState")]
        public ExportOptions ExportState { get; set; } = ExportOptions.NotStarted;

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

        [BsonIgnoreIfNull]
        [BsonElement("lastCaptureAt")]
        public BsonDateTime LastCaptureAt { get; set; } = null;
        #pragma warning restore SA1516
    }

    // See Startup.cs for the code on how this is serlizalized
    #pragma warning disable SA1201
    public enum ExportOptions
    {
        NotStarted,
        Pending,
        Done,
        Error,
    }
    #pragma warning restore SA1201

    public sealed class SessionRepository : RepositoryBase<SessionSchema>
    {
        public SessionRepository(IMongoDatabase database)
            : base(database, "sessions")
        {
        }

        public override async Task<SessionSchema> FindById(string id)
        {
            var filter = Builders<SessionSchema>.Filter.Where(s => s.Id == id);

            var sessionDoc = await this.Collection
                .Find(filter)
                .FirstOrDefaultAsync();

            return sessionDoc;
        }
    }

    public sealed class CapturesRepository : RepositoryBase<BsonDocument>
    {
        // The session Id isn't know until runtime,
        // So it is constructed as temp
        public CapturesRepository(IMongoDatabase database)
            : base(database, "sessions.this.is.temp")
        {
        }
    }
}