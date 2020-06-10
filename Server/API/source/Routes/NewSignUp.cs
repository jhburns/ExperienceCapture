namespace Carter.App.Route.NewSignUp
{
    using Carter;

    using Carter.App.Lib.Authentication;
    using Carter.App.Lib.Environment;
    using Carter.App.Lib.Generate;
    using Carter.App.Lib.Network;
    using Carter.App.Lib.Repository;
    using Carter.App.Lib.Timer;

    using Carter.App.Route.PreSecurity;
    using Carter.App.Route.Users;

    using Carter.App.Validation.AdminPassword;

    using Carter.ModelBinding;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;

    public class NewSignUp : CarterModule
    {
        public NewSignUp(
            IRepository<AccessTokenSchema> accessRepo,
            IRepository<SignUpTokenSchema> signUpRepo,
            IAppEnvironment env,
            IDateExtra date)
            : base("/users")
        {
            // TODO: only allow admins to create sign-up tokens, or another restriction
            this.Before += PreSecurity.GetSecurityCheck(accessRepo, date);

            this.Post("/signUp", async (req, res) =>
            {
                string newToken = Generate.GetRandomToken();

                var tokenDoc = new SignUpTokenSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Hash = PasswordHasher.Hash(newToken),
                    CreatedAt = new BsonDateTime(date.Now),
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

            this.Post("/signUp/admin/", async (req, res) =>
            {
                var newAdmin = await req.BindAndValidate<AdminPassword>();
                if (!newAdmin.ValidationResult.IsValid)
                {
                    res.StatusCode = 400;
                    return;
                }

                if (!PasswordHasher.Check(newAdmin.Data.password, env.PasswordHash))
                {
                    res.StatusCode = 401;
                    return;
                }

                string newToken = Generate.GetRandomToken();

                var tokenDoc = new SignUpTokenSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Hash = PasswordHasher.Hash(newToken),
                    CreatedAt = new BsonDateTime(date.Now),
                };

                await signUpRepo.Add(tokenDoc);

                var responce = new ClaimTokenResponce
                {
                    ClaimToken = newToken,
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