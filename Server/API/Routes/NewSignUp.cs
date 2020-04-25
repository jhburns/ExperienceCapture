namespace Carter.App.Route.NewSignUp
{
    using System;

    using Carter;

    using Carter.App.Lib.Authentication;
    using Carter.App.Lib.Generate;
    using Carter.App.Lib.Network;

    using Carter.App.Route.PreSecurity;

    using MongoDB.Bson;
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
                var signUpTokens = db.GetCollection<BsonDocument>("users.tokens.signUp");

                var tokenDoc = new
                {
                    hash = PasswordHasher.Hash(newToken),
                    expirationSeconds = 86400, // One day
                    createdAt = new BsonDateTime(DateTime.Now),
                };

                await signUpTokens.InsertOneAsync(tokenDoc.ToBsonDocument());

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
        #pragma warning disable SA1516, SA1300
        public string hash { get; set; }
        public int expirationSeconds { get; set; }
        public BsonDateTime createdAt { get; set; }
        #pragma warning restore SA151, SA1300
    }
}