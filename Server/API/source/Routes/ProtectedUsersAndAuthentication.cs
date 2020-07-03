namespace Carter.App.Route.ProtectedUsersAndAuthentication
{
    using Carter;

    using Carter.App.Lib.Authentication;
    using Carter.App.Lib.Environment;
    using Carter.App.Lib.Generate;
    using Carter.App.Lib.Network;
    using Carter.App.Lib.Repository;
    using Carter.App.Lib.Timer;

    using Carter.App.Route.PreSecurity;
    using Carter.App.Route.UsersAndAuthentication;

    using Carter.Request;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;

    using static Microsoft.AspNetCore.Http.StatusCodes;

    public class ProtectedUsersAndAuthentication : CarterModule
    {
        public ProtectedUsersAndAuthentication(
            IRepository<AccessTokenSchema> accessRepo,
            IRepository<SignUpTokenSchema> signUpRepo,
            IRepository<PersonSchema> personRepo,
            IAppEnvironment env,
            IDateExtra date)
            : base("/")
        {
            this.Before += PreSecurity.CheckAccess(accessRepo, date);

            this.Get("users/{id}/", async (req, res) =>
            {
                string userID = req.RouteValues.As<string>("id");
                var person = await personRepo.FindById(userID);

                if (person == null)
                {
                    res.StatusCode = Status404NotFound;
                    return;
                }

                person.InternalId = null;

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

                await res.FromBson(person);
            });

            this.Delete("users/{id}/", async (req, res) =>
            {
                string userID = req.RouteValues.As<string>("id");
                var person = await personRepo.FindById(userID);

                if (person == null)
                {
                    res.StatusCode = Status404NotFound;
                    return;
                }

                person.InternalId = null;

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

            this.Post("authentication/signUps", async (req, res) =>
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

    public class SignUpTokenSchema
    {
        #pragma warning disable SA1516
        [BsonId]
        public BsonObjectId InternalId { get; set; }

        [BsonElement("hash")]
        public string Hash { get; set; }

        [BsonElement("expirationSeconds")]
        public int ExpirationSeconds { get; set; } = 86400; // One day

        [BsonElement("createdAt")]
        public BsonDateTime CreatedAt { get; set; }

        [BsonElement("role")]
        public RoleOptions Role { get; set; } = RoleOptions.Normal;
        #pragma warning restore SA1516
    }

    public sealed class SignUpTokenRepository : RepositoryBase<SignUpTokenSchema>
    {
        public SignUpTokenRepository(IMongoDatabase database)
            : base(database, "persons.tokens.signUps")
        {
        }
    }

    public class SignUpTokenResponce
    {
        #pragma warning disable SA1516
        [BsonRequired]
        [BsonElement("signUpToken")]
        public string SignUpToken { get; set; }

        [BsonRequired]
        [BsonElement("expiration")]
        public BsonDateTime Expiration { get; set; }
        #pragma warning restore SA1516
    }
}