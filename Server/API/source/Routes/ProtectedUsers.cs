namespace Carter.App.Route.ProtectedUsers
{
    using System.Collections.Generic;
    using System.Linq;

    using Carter;

    using Carter.App.Lib.Authentication;
    using Carter.App.Lib.Environment;
    using Carter.App.Lib.Generate;
    using Carter.App.Lib.Network;
    using Carter.App.Lib.Repository;
    using Carter.App.Lib.Timer;

    using Carter.App.Route.PreSecurity;
    using Carter.App.Route.Users;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;

    public class ProtectedUsers : CarterModule
    {
        public ProtectedUsers(
            IRepository<AccessTokenSchema> accessRepo,
            IRepository<SignUpTokenSchema> signUpRepo,
            IRepository<PersonSchema> personRepo,
            IAppEnvironment env,
            IDateExtra date)
            : base("/")
        {
            this.Before += PreSecurity.CheckAccess(accessRepo, date, RoleOptions.Admin);

            this.Get("users", async (req, res) =>
            {
                var filter = Builders<PersonSchema>.Filter.Empty;
                var sorter = Builders<PersonSchema>.Sort
                    .Descending(p => p.Fullname);

                var persons = await personRepo.FindAll(filter, sorter);

                var personsWithoutId = persons.Select((p) =>
                {
                    p.InternalId = null;
                    return p;
                });

                var responceBody = new PersonsResponce
                {
                    ContentList = new List<PersonSchema>(personsWithoutId),
                };

                string json = JsonQuery.FulfilEncoding(req.Query, responceBody);
                if (json != null)
                {
                    await res.FromJson(json);
                    return;
                }

                await res.FromBson(responceBody);
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

    public class PersonsResponce
    {
        #pragma warning disable SA1516
        [BsonElement("contentList")]
        public List<PersonSchema> ContentList { get; set; }
        #pragma warning restore SA1516
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