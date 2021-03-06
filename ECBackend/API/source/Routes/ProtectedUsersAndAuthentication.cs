namespace Carter.App.Route.ProtectedUsersAndAuthentication
{
    using Carter;

    using Carter.App.Libs.Authentication;
    using Carter.App.Libs.Environment;
    using Carter.App.Libs.Generate;
    using Carter.App.Libs.Network;
    using Carter.App.Libs.Repository;
    using Carter.App.Libs.Timer;

    using Carter.App.MetaData.UsersAndAuthentication;

    using Carter.App.Route.PreSecurity;
    using Carter.App.Route.UsersAndAuthentication;

    using Carter.Request;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;

    using static Microsoft.AspNetCore.Http.StatusCodes;

    /// <summary>
    /// User and Authentication routes that only users can access.
    /// </summary>
    public class ProtectedUsersAndAuthentication : CarterModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtectedUsersAndAuthentication"/> class.
        /// </summary>
        /// <param name="accessRepo">Supplied through DI.</param>
        /// <param name="signUpRepo">Supplied through DI.</param>
        /// <param name="personRepo">Supplied through DI.</param>
        /// <param name="env">Supplied through DI.</param>
        /// <param name="date">Supplied through DI.</param>
        public ProtectedUsersAndAuthentication(
            IRepository<AccessTokenSchema> accessRepo,
            IRepository<SignUpTokenSchema> signUpRepo,
            IRepository<PersonSchema> personRepo,
            IAppEnvironment env,
            IDateExtra date)
            : base("/")
        {
            this.Before += PreSecurity.CheckAccess(accessRepo, date);

            this.Get<GetUser>("users/{id}/", async (req, res) =>
            {
                string userID = req.RouteValues.As<string>("id");
                var person = await personRepo.FindById(userID);

                if (person == null || !person.IsExisting)
                {
                    res.StatusCode = Status404NotFound;
                    return;
                }

                // Has to exit due to pre-security check
                string token = req.Cookies["ExperienceCapture-Access-Token"];
                var accessToken = await accessRepo.FindOne(
                    Builders<AccessTokenSchema>
                        .Filter
                        .Where(a => a.Hash == PasswordHasher.Hash(token)));

                if (person.InternalId != accessToken.User && accessToken.Role != RoleOptions.Admin)
                {
                    res.StatusCode = Status401Unauthorized;
                    return;
                }

                string json = JsonQuery.FulfilEncoding(req.Query, person);
                if (json != null)
                {
                    await res.FromJson(json);
                    return;
                }

                person.InternalId = null;

                await res.FromBson(person);
            });

            this.Delete<DeleteUser>("users/{id}/", async (req, res) =>
            {
                string userID = req.RouteValues.As<string>("id");
                var person = await personRepo.FindById(userID);

                if (person == null)
                {
                    res.StatusCode = Status404NotFound;
                    return;
                }

                // Has to exit due to pre-security check
                string token = req.Cookies["ExperienceCapture-Access-Token"];
                var accessFilter = Builders<AccessTokenSchema>
                        .Filter
                        .Where(a => a.Hash == PasswordHasher.Hash(token));
                var accessToken = await accessRepo.FindOne(accessFilter);

                // Check if the user being requested is the same
                // As the one requesting, unless they have the admin role
                if (person.InternalId != accessToken.User && accessToken.Role != RoleOptions.Admin)
                {
                    res.StatusCode = Status401Unauthorized;
                    return;
                }

                var filter = Builders<PersonSchema>.Filter
                    .Where(p => p.Id == userID);

                var update = Builders<PersonSchema>.Update
                    .Set(p => p.IsExisting, false);

                await personRepo.Update(filter, update);

                await res.FromString();
            });

            this.Post<PostSignUp>("authentication/signUps", async (req, res) =>
            {
                string newToken = Generate.GetRandomToken();

                var tokenDoc = new SignUpTokenSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Hash = PasswordHasher.Hash(newToken),
                    CreatedAt = new BsonDateTime(date.UtcNow),
                };

                await signUpRepo.Add(tokenDoc);

                var responce = new SignUpTokenResponce
                {
                    SignUpToken = newToken,
                    Expiration = TimerExtra.ProjectSeconds(date, tokenDoc.ExpirationSeconds),
                };
                var responceDoc = responce.ToBsonDocument();

                string json = JsonQuery.FulfilEncoding(req.Query, responceDoc);
                if (json != null)
                {
                    await res.FromJson(json);
                    return;
                }

                await res.FromBson(responceDoc);
            });
        }
    }

    /// <summary>
    /// Database schema for a sign up token.
    /// </summary>
    public class SignUpTokenSchema
    {
        #pragma warning disable SA1516
        /// <summary>Id in MongoDB.</summary>
        [BsonId]
        public BsonObjectId InternalId { get; set; }

        /// <summary>A sign up token's hash.</summary>
        [BsonElement("hash")]
        public string Hash { get; set; }

        /// <summary>How long for the token to be valid.</summary>
        [BsonElement("expirationSeconds")]
        public int ExpirationSeconds { get; set; } = 86400; // One day

        /// <summary>When the token was created.</summary>
        [BsonElement("createdAt")]
        public BsonDateTime CreatedAt { get; set; }

        /// <summary>The role to grant the new user.</summary>
        [BsonElement("role")]
        public RoleOptions Role { get; set; } = RoleOptions.Normal;

        /// <summary>Whether the token should be considered deleted.</summary>
        [BsonElement("isExisting")]
        public bool IsExisting { get; set; } = true;
        #pragma warning restore SA1516
    }

    /// <inheritdoc />
    public sealed class SignUpTokenRepository : RepositoryBase<SignUpTokenSchema>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignUpTokenRepository"/> class.
        /// </summary>
        /// <param name="database">A MongoDB database connection.</param>
        public SignUpTokenRepository(IMongoDatabase database)
            : base(database, "persons.tokens.signUps")
        {
        }
    }

    /// <summary>
    /// Responce schema for a sign up token.
    /// </summary>
    public class SignUpTokenResponce
    {
        #pragma warning disable SA1516
        /// <summary>Base64 encoded.</summary>
        [BsonRequired]
        [BsonElement("signUpToken")]
        public string SignUpToken { get; set; }

        /// <summary>UTC timestamp until the token is no longer valid.</summary>
        [BsonRequired]
        [BsonElement("expiration")]
        public BsonDateTime Expiration { get; set; }
        #pragma warning restore SA1516
    }
}