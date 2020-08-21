namespace Carter.App.Route.Sessions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Carter;

    using Carter.App.Libs.Authentication;
    using Carter.App.Libs.Generate;
    using Carter.App.Libs.Network;
    using Carter.App.Libs.Repository;
    using Carter.App.Libs.Timer;

    using Carter.App.MetaData.Sessions;

    using Carter.App.Route.PreSecurity;
    using Carter.App.Route.UsersAndAuthentication;

    using Carter.Request;

    using Microsoft.Extensions.Logging;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;

    using static Microsoft.AspNetCore.Http.StatusCodes;

    /// <summary>
    /// Session routes.
    /// </summary>
    public class Sessions : CarterModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sessions"/> class.
        /// </summary>
        /// <param name="accessRepo">Supplied through DI.</param>
        /// <param name="sessionRepo">Supplied through DI.</param>
        /// <param name="personRepo">Supplied through DI.</param>
        /// <param name="captureRepo">Supplied through DI.</param>
        /// <param name="logger">Supplied through DI.</param>
        /// <param name="date">Supplied through DI.</param>
        public Sessions(
            IRepository<AccessTokenSchema> accessRepo,
            IRepository<SessionSchema> sessionRepo,
            IRepository<PersonSchema> personRepo,
            IRepository<BsonDocument> captureRepo,
            ILogger logger,
            IDateExtra date)
            : base("/sessions")
        {
            this.Before += PreSecurity.CheckAccess(accessRepo, date);

            this.Post<PostSessions>("/", async (req, res) =>
            {
                // Has to exist due to PreSecurity Check
                string token = req.Cookies["ExperienceCapture-Access-Token"];

                var accessTokenDoc = await accessRepo.FindOne(
                    Builders<AccessTokenSchema>
                        .Filter
                        .Where(a => a.Hash == PasswordHasher.Hash(token)));

                var filterUser = Builders<PersonSchema>.Filter.Where(p => p.InternalId == accessTokenDoc.User);

                var user = await personRepo
                    .FindOne(filterUser);

                string shortID = Generate.GetRandomId(4);

                var sessionDoc = new SessionSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Id = shortID,
                    User = user, // Copying user data instead of referencing so it can never change in the session
                    CreatedAt = new BsonDateTime(date.UtcNow),
                    Tags = new List<string>(),
                };

                // Retry generating a short id until it is unique
                bool isUnique = true;
                do
                {
                    try
                    {
                        sessionDoc.Id = shortID;
                        await sessionRepo.Add(sessionDoc);
                        isUnique = true;
                    }
                    catch (MongoWriteException e)
                    {
                        // Re-throw any other type of exception except non-unique keys
                        if (e.WriteError.Code != 11000)
                        {
                            throw e;
                        }

                        shortID = Generate.GetRandomId(4);
                        isUnique = false;
                    }
                }
                while (!isUnique);

                // isOngoing is a proxy variable and will always start out as true
                sessionDoc.IsOngoing = true;
                sessionDoc.InternalId = null;
                sessionDoc.User.InternalId = null;

                captureRepo.Configure($"sessions.{shortID}");

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

            this.Get<GetSessions>("/", async (req, res) =>
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
                            & ((builder.Eq(s => s.LastCaptureAt, BsonNull.Value)
                            & builder.Where(s => s.CreatedAt > startMin))
                            | (builder.Eq(s => s.LastCaptureAt, BsonNull.Value)
                            & builder.Where(s => s.LastCaptureAt > closeMin)));
                    }
                    else
                    {
                        filter &= builder.Where(s => s.IsOpen == false)
                            | ((builder.Eq(s => s.LastCaptureAt, BsonNull.Value)
                            & builder.Where(s => s.CreatedAt < startMin))
                            | (builder.Eq(s => s.LastCaptureAt, BsonNull.Value)
                            & builder.Where(s => s.LastCaptureAt < closeMin)));
                    }
                }

                var page = req.Query.As<int?>("page") ?? 1;
                if (page < 1)
                {
                    // Page query needs to be possible
                    res.StatusCode = Status400BadRequest;
                    return;
                }

                var direction = req.Query.As<string>("sort");
                SortDefinition<SessionSchema> sorter;
                if (direction == null)
                {
                    sorter = Builders<SessionSchema>.Sort.Descending(s => s.CreatedAt);
                }
                else
                {
                    if (Enum.TryParse(typeof(SortOptions), direction, true, out object options))
                    {
                        sorter = ((SortOptions)options).ToDefinition();
                    }
                    else
                    {
                        res.StatusCode = Status400BadRequest;
                        return;
                    }
                }

                var sessionDocs = await sessionRepo
                    .FindAll(filter, sorter, page);

                var sessionsDocsWithOngoing = sessionDocs.Select((s) =>
                {
                    bool isStarted = false;
                    if (s.LastCaptureAt != BsonNull.Value)
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

            this.Post<PostSession>("/{id}", async (req, res) =>
            {
                string shortID = req.RouteValues.As<string>("id");

                var sessionDoc = await sessionRepo
                    .FindById(shortID);

                if (sessionDoc == null)
                {
                    res.StatusCode = Status404NotFound;
                    return;
                }

                if (!sessionDoc.IsOpen)
                {
                    res.StatusCode = Status400BadRequest;
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
                            res.StatusCode = Status400BadRequest;
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
                        res.StatusCode = Status400BadRequest;
                        return;
                    }
                }

                // Manual validation, because Fluent Validation would remove extra properties
                if (!document.Contains("frameInfo")
                    || document["frameInfo"].BsonType != BsonType.Document
                    || !document["frameInfo"].AsBsonDocument.Contains("realtimeSinceStartup")
                    || document["frameInfo"]["realtimeSinceStartup"].BsonType != BsonType.Double)
                {
                    res.StatusCode = Status400BadRequest;
                    return;
                }

                captureRepo.Configure($"sessions.{shortID}");
                await captureRepo.Add(document);

                var filter = Builders<SessionSchema>.Filter.Where(s => s.Id == shortID);

                // This lastCaptureAt is undefined on the session document until the first call of this endpoint
                // Export flags are reset so the session can be re-exported
                var update = Builders<SessionSchema>.Update
                    .Set(s => s.LastCaptureAt, new BsonDateTime(date.UtcNow))
                    .Set(s => s.ExportState, ExportOptions.NotStarted);

                await sessionRepo.Update(filter, update);

                await res.FromString();
            });

            this.Get<GetSession>("/{id}", async (req, res) =>
            {
                string shortID = req.RouteValues.As<string>("id");

                var sessionDoc = await sessionRepo
                    .FindById(shortID);

                if (sessionDoc == null)
                {
                    res.StatusCode = Status404NotFound;
                    return;
                }

                var startRange = new BsonDateTime(date.UtcNow.AddSeconds(-300)); // 5 minutes
                var closeRange = new BsonDateTime(date.UtcNow.AddSeconds(-5)); // 5 seconds
                bool isStarted = false;

                // Check if key exists
                if (sessionDoc.LastCaptureAt != BsonNull.Value)
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

            this.Delete<DeleteSession>("/{id}", async (req, res) =>
            {
                string shortID = req.RouteValues.As<string>("id");

                var filter = Builders<SessionSchema>.Filter.Where(s => s.Id == shortID);
                var sessionDoc = await sessionRepo
                    .FindById(shortID);

                if (sessionDoc == null)
                {
                    res.StatusCode = Status404NotFound;
                    return;
                }

                var update = Builders<SessionSchema>.Update
                    .Set(s => s.IsOpen, false);

                await sessionRepo.Update(filter, update);
                await res.FromString();
            });
        }
    }

    /// <summary>
    /// Database schema for a session.
    /// </summary>
    public class SessionSchema
    {
        #pragma warning disable SA1516
        /// <summary>Id in MongoDB.</summary>
        [BsonIgnoreIfNull]
        [BsonId]
        public BsonObjectId InternalId { get; set; }

        /// <summary>A human usable unique value.</summary>
        [BsonElement("id")]
        public string Id { get; set; }

        /// <summary>Whether the session was deleted yet.</summary>
        [BsonElement("isOpen")]
        public bool IsOpen { get; set; } = true;

        /// <summary>Progress towards the session being exported.</summary>
        [BsonElement("exportState")]
        public ExportOptions ExportState { get; set; } = ExportOptions.NotStarted;

        // Copying user data instead of referencing so it can never change with the session
        // Also so that it is easy to include when exporting

        /// <summary>A copy the user who created this session's information.</summary>
        [BsonElement("user")]
        public PersonSchema User { get; set; }

        /// <summary>When the session was created.</summary>
        [BsonElement("createdAt")]
        public BsonDateTime CreatedAt { get; set; }

        /// <summary>Optional metadata.</summary>
        [BsonElement("tags")]
        public List<string> Tags { get; set; }

        // This is a proxy-property, and should only
        // Be set when returned

        /// <summary>Whether there has been any recent captured added to this session.</summary>
        [BsonIgnoreIfNull]
        [BsonElement("isOngoing")]
        public bool? IsOngoing { get; set; } = null;

        // Is really type 'BsonDateTime', but needs to be
        // A BsonValue in order to serialize null properly
        // See: https://jira.mongodb.org/browse/CSHARP-863

        /// <summary>Datetime of when the last capture was added.</summary>
        [BsonElement("lastCaptureAt")]
        public BsonValue LastCaptureAt { get; set; } = BsonNull.Value;
        #pragma warning restore SA1516
    }

    /// <summary>
    /// Responce schema for a list of sessions.
    /// </summary>
    public class SessionsResponce
    {
        #pragma warning disable SA1516

        /// <summary>Ordered group of sessions.</summary>
        [BsonElement("contentList")]
        public List<SessionSchema> ContentList { get; set; }

        /// <summary>How many pages exist for this query.</summary>
        [BsonElement("pageTotal")]
        public long PageTotal { get; set; }

        #pragma warning restore SA1516
    }

    // See Startup.cs for the code on how this is serlizalized

    /// <summary>
    /// Possible states of a session export.
    /// </summary>
    #pragma warning disable SA1201
    public enum ExportOptions
    {
        /// <summary>When the export is started.</summary>
        NotStarted,

        /// <summary>When the session is being exported</summary>
        Pending,

        /// <summary>When the session can be downloaded</summary>
        Done,

        /// <summary>When the session export needs debugging</summary>
        Error,
    }
    #pragma warning restore SA1201

    #pragma warning disable SA1201
    /// <summary>
    /// Directions to sort in.
    /// </summary>
    public enum SortOptions
    {
        /// <summary>0-9, A-Z. But session ids have to always start with a letter</summary>
        Alphabetical,

        /// <summary>Datetime descending</summary>
        NewestFirst,

        /// <summary>Datetime ascending</summary>
        OldestFirst,
    }
    #pragma warning restore SA1201

    /// <inheritdoc />
    public sealed class SessionRepository : RepositoryBase<SessionSchema>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionRepository"/> class.
        /// </summary>
        /// <param name="database">A MongoDB database connection.</param>
        public SessionRepository(IMongoDatabase database)
            : base(database, "sessions")
        {
            var index = Builders<SessionSchema>.IndexKeys;

            var keyCreated = index.Ascending(s => s.CreatedAt);
            _ = this.Index(keyCreated);

            var keyId = index.Ascending(s => s.Id);
            _ = this.Index(keyId, new CreateIndexOptions<SessionSchema>
            {
                Unique = true,
            });
        }

        /// <inheritdoc />
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

        /// <summary>
        /// Custom implementation to be session oriented.
        /// </summary>
        /// <returns>
        /// A document.
        /// </returns>
        /// <param name="id">An id to query the database for.</param>
        public override async Task<SessionSchema> FindById(string id)
        {
            var filter = Builders<SessionSchema>.Filter.Where(s => s.Id == id);

            var sessionDoc = await this.Collection
                .Find(filter)
                .FirstOrDefaultAsync();

            return sessionDoc;
        }
    }

    /// <inheritdoc />
    public sealed class CapturesRepository : RepositoryBase<BsonDocument>
    {
        // The session Id isn't know until runtime,
        // So it is constructed as temp

        /// <summary>
        /// Initializes a new instance of the <see cref="CapturesRepository"/> class.
        /// </summary>
        /// <param name="database">A MongoDB database connection.</param>
        public CapturesRepository(IMongoDatabase database)
            : base(database, "sessions.this.is.temp")
        {
        }
    }

    /// <summary>
    /// Sorting helper that maps options to sorters.
    /// </summary>
    internal static class SortExtra
    {
        public static SortDefinition<SessionSchema> ToDefinition(this SortOptions option)
        {
            switch (option)
            {
                case SortOptions.Alphabetical: return Builders<SessionSchema>.Sort.Ascending(s => s.Id);
                case SortOptions.NewestFirst: return Builders<SessionSchema>.Sort.Descending(s => s.CreatedAt);
                case SortOptions.OldestFirst: return Builders<SessionSchema>.Sort.Ascending(s => s.CreatedAt);
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}