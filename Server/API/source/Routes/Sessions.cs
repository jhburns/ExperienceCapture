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
    using Carter.App.Lib.Timer;

    using Carter.App.Route.PreSecurity;
    using Carter.App.Route.Users;

    using Carter.Request;

    using Microsoft.Extensions.Logging;

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
            IRepository<BsonDocument> captureRepo,
            ILogger logger,
            IDateExtra date)
            : base("/sessions")
        {
            this.Before += PreSecurity.GetSecurityCheck(accessRepo, date);

            this.Post("/", async (req, res) =>
            {
                string uniqueID = Generate.GetRandomId(4);

                // Will loop until a unique id is found
                // Needed because the ids that are generated are from a small number of combinations
                while ((await sessionRepo.FindById(uniqueID)) != null)
                {
                    uniqueID = Generate.GetRandomId(4);
                }

                // Has to exist due to PreSecurity Check
                string token = req.Cookies["ExperienceCapture-Access-Token"];

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
                    CreatedAt = new BsonDateTime(date.UtcNow),
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

                // Note: only use `&=` for adding to the filter,
                // Or else the filter cannot handle multiple query string options
                FilterDefinition<SessionSchema> filter = builder.Empty;

                var startMin = new BsonDateTime(date.UtcNow.AddSeconds(-300)); // 5 minutes
                var closeMin = new BsonDateTime(date.UtcNow.AddSeconds(-5)); // 5 seconds

                var hasTags = req.Query.AsMultiple<string>("hasTags").ToList();
                if (hasTags.Count > 0)
                {
                    foreach (var tag in hasTags)
                    {
                        filter &= builder.Where(s => s.Tags.Contains(tag));
                    }
                }

                var lacksTags = req.Query.AsMultiple<string>("lacksTags").ToList();
                if (lacksTags.Count > 0)
                {
                    foreach (var tag in lacksTags)
                    {
                        filter &= builder.Where(s => !s.Tags.Contains(tag));
                    }
                }

                // Three potential options: null, true, or false
                if (req.Query.As<bool?>("isOngoing") != null)
                {
                    bool isOngoing = req.Query.As<bool>("isOngoing");

                    if (isOngoing)
                    {
                        filter &= builder.Where(s => s.IsOpen == true)
                            & ((builder.Exists(s => s.LastCaptureAt, false)
                            & builder.Where(s => s.CreatedAt > startMin))
                            | (builder.Exists(s => s.LastCaptureAt, true)
                            & builder.Where(s => s.LastCaptureAt > closeMin)));
                    }
                    else
                    {
                        filter &= builder.Where(s => s.IsOpen == false)
                            | ((builder.Exists(s => s.LastCaptureAt, false)
                            & builder.Where(s => s.CreatedAt < startMin))
                            | (builder.Exists(s => s.LastCaptureAt, true)
                            & builder.Where(s => s.LastCaptureAt < closeMin)));
                    }
                }

                var page = req.Query.As<int?>("page") ?? 1;
                if (page < 1)
                {
                    // Page query needs to be possible
                    res.StatusCode = 400;
                    return;
                }

                var sorter = Builders<SessionSchema>.Sort.Descending(s => s.CreatedAt);
                var sessionDocs = await sessionRepo
                    .FindAll(filter, sorter, page);

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
                            && startMin < s.CreatedAt)
                            || (isStarted
                            && closeMin < s.LastCaptureAt);
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

                var count = await sessionRepo.FindThenCount(filter);
                var clientValues = new SessionsResponce
                {
                    // Bson documents can't start with an array like Json, so a wrapping object is used instead
                    ContentList = sessionsDocsWithOngoing.ToList(),
                    PageTotal = (long)Math.Ceiling((double)count / 10d),
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
                        catch (Exception err)
                        {
                            logger.LogError(err.Message);
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
                    catch (Exception err)
                    {
                        logger.LogError(err.Message);
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
                    .Set(s => s.LastCaptureAt, new BsonDateTime(date.UtcNow))
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

                var startRange = new BsonDateTime(date.UtcNow.AddSeconds(-300)); // 5 minutes
                var closeRange = new BsonDateTime(date.UtcNow.AddSeconds(-5)); // 5 seconds
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
                sessionDoc.InternalId = null;
                sessionDoc.User.InternalId = null;

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

        // This is a proxy-property, and should only
        // Be set when returned
        [BsonIgnoreIfNull]
        [BsonElement("isOngoing")]
        public bool? IsOngoing { get; set; } = null;

        // Because this property doesn't exist when null
        // Check with the .Exists(...) function,
        // Not .Where(.. == null)
        [BsonIgnoreIfNull]
        [BsonElement("lastCaptureAt")]
        public BsonDateTime LastCaptureAt { get; set; } = null;
        #pragma warning restore SA1516
    }

    public class SessionsResponce
    {
        #pragma warning disable SA1516
        [BsonElement("contentArray")]
        public List<SessionSchema> ContentList { get; set; }

        [BsonElement("pageTotal")]
        public long PageTotal { get; set; }

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
            var index = Builders<SessionSchema>.IndexKeys;
            var keyCreated = index.Ascending(s => s.CreatedAt);

            _ = this.Index(keyCreated);

            var keyId = index.Ascending(s => s.Id);

            _ = this.Index(keyId);
        }

        public override async Task<IList<SessionSchema>> FindAll(
            FilterDefinition<SessionSchema> filter,
            SortDefinition<SessionSchema> sorter,
            int page)
        {
            var projection = Builders<SessionSchema>.Projection
                .Exclude(s => s.InternalId);

            int limit = 10;

            return await this.Collection
                .Find(filter)
                .Skip((page - 1) * limit)
                .Limit(limit)
                .Sort(sorter)
                .Project<SessionSchema>(projection)
                .ToListAsync();
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