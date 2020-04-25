namespace Carter.App.Route.NewSignUp
{
    using System;

    using Carter;

    using Carter.App.Lib.Authentication;
    using Carter.App.Lib.Generate;
    using Carter.App.Lib.Network;

    using Carter.App.Route.PreSecurity;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;

    public class NewSignUp : CarterModule
    {
        public NewSignUp(IMongoDatabase db)
            : base("/users")
        {
            // TODO: only allow admins to create sign-up tokens, or another restriction
            this.Before += PreSecurity.GetSecurityCheck(db);

            this.Post("/signUp/", async (req, res) =>
            {
                string newToken = Generate.GetRandomToken();
                var signUpTokens = db.GetCollection<SignUpTokenSchema>(SignUpTokenSchema.CollectionName);

                var tokenDoc = new SignUpTokenSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Hash = PasswordHasher.Hash(newToken),
                    CreatedAt = new BsonDateTime(DateTime.Now),
                };

                await signUpTokens.InsertOneAsync(tokenDoc);

                var responce = new
                {
                    signUpToken = newToken,
                };
                var responceDoc = responce.ToBsonDocument();

                string json = JsonQuery.FulfilEncoding(req.Query, responceDoc);
                if (json != null)
                {
                    JsonResponce.FromString(res, json);
                    return;
                }

                BsonResponse.FromDoc(res, responceDoc);
            });
        }
    }

    public class SignUpTokenSchema
    {
        #pragma warning disable SA1516
        [BsonIgnore]
        public const string CollectionName = "persons.tokens.signUps";

        [BsonId]
        public BsonObjectId InternalId { get; set; }

        [BsonElement("hash")]
        public string Hash { get; set; }

        [BsonElement("expirationSeconds")]
        public int ExpirationSeconds { get; set; } = 86400; // One day

        [BsonElement("createdAt")]
        public BsonDateTime CreatedAt { get; set; }

        #pragma warning restore SA151
    }
}