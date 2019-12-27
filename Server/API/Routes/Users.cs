namespace Carter.App.Route.Users
{
    using System;

    using Carter;

    using Carter.App.Lib.Authentication;
    using Carter.App.Lib.Generate;
    using Carter.App.Lib.Mongo;
    using Carter.App.Lib.Timer;

    using Carter.App.Validation.AccessTokenRequest;
    using Carter.App.Validation.AdminPassword;
    using Carter.App.Validation.Person;

    using Carter.ModelBinding;
    using Carter.Request;

    using Microsoft.AspNetCore.Http;

    using MongoDB.Bson;
    using MongoDB.Driver;

    public class Users : CarterModule
    {
        private static readonly string PasswordHash = Environment.GetEnvironmentVariable("admin_password_hash");

        public Users(IMongoDatabase db)
            : base("/users")
        {
            this.Post("/", async (req, res) =>
            {
                var newPerson = await req.BindAndValidate<Person>();

                if (!newPerson.ValidationResult.IsValid)
                {
                    res.StatusCode = 400;
                    return;
                }

                var person = await GoogleApi.ValidateUser(newPerson.Data.idToken);
                if (person == null)
                {
                    res.StatusCode = 401;
                    return;
                }

                var signUpTokens = db.GetCollection<BsonDocument>("users.tokens.signUp");

                var signUpDoc = await signUpTokens.FindEqAsync("hash", PasswordHasher.Hash(newPerson.Data.signUpToken));

                if (signUpDoc == null || signUpDoc["createdAt"].IsAfter(signUpDoc["expirationSeconds"]))
                {
                    res.StatusCode = 401;
                    return;
                }

                var users = db.GetCollection<BsonDocument>("users");

                var existingPerson = await users.FindEqAsync("id", person.Subject);
                if (existingPerson != null)
                {
                    res.StatusCode = 409;
                    return;
                }

                BsonDocument personDoc = new BsonDocument()
                {
                    { "id", person.Subject },
                    { "fullname", person.Name },
                    { "firstname", person.GivenName },
                    { "lastname", person.FamilyName },
                    { "email", person.Email },
                    { "createdAt", new BsonDateTime(DateTime.Now) },
                };

                await users.InsertOneAsync(personDoc);
                await res.WriteAsync("OK");
            });

            this.Post("/{id}/tokens/", async (req, res) =>
            {
                var users = db.GetCollection<BsonDocument>("users");

                string userID = req.RouteValues.As<string>("id");
                var userDoc = await users.FindEqAsync("id", userID);

                if (userDoc == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                var newAccessRequest = await req.BindAndValidate<AccessTokenRequest>();
                if (!newAccessRequest.ValidationResult.IsValid)
                {
                    res.StatusCode = 400;
                    return;
                }

                if (await GoogleApi.ValidateUser(newAccessRequest.Data.idToken) == null)
                {
                    res.StatusCode = 401;
                    return;
                }

                string newToken = Generate.GetRandomToken();
                string newHash = PasswordHasher.Hash(newToken);
                var accessTokens = db.GetCollection<BsonDocument>("users.tokens.access");

                var tokenObject = new
                {
                    hash = newHash,
                    user = userDoc["_id"],
                    expirationSeconds = 259200, // Three days
                    createdAt = new BsonDateTime(DateTime.Now),
                };

                BsonDocument tokenDoc = tokenObject.ToBsonDocument();

                await accessTokens.InsertOneAsync(tokenDoc);

                if (newAccessRequest.Data.claimToken != null)
                {
                    var claimTokens = db.GetCollection<BsonDocument>("users.tokens.claim");

                    var filterClaims = Builders<BsonDocument>.Filter.Eq("hash", PasswordHasher.Hash(newAccessRequest.Data.claimToken));
                    var claimDoc = await claimTokens.Find(filterClaims).FirstOrDefaultAsync();

                    if (claimDoc == null || claimDoc["createdAt"].IsAfter(claimDoc["expirationSeconds"]))
                    {
                        res.StatusCode = 401;
                        return;
                    }

                    // Don't allow overwriting an access token
                    if (claimDoc["isPending"].AsBoolean && claimDoc["isExisting"].AsBoolean)
                    {
                        var update = Builders<BsonDocument>.Update
                            .Set("isPending", false)
                            .Set("access", tokenDoc["_id"])
                            .Set("accessToken", newToken);
                        await claimTokens.UpdateOneAsync(filterClaims, update);
                    }

                    await res.WriteAsync("OK");
                }
                else
                {
                    await res.WriteAsync(newHash);
                }
            });

            this.Post("/claims/", async (req, res) =>
            {
                string newToken = Generate.GetRandomToken();
                string newHash = PasswordHasher.Hash(newToken);
                var accessTokens = db.GetCollection<BsonDocument>("users.tokens.claim");

                var tokenDoc = new
                {
                    hash = newHash,
                    expirationSeconds = 3600, // One hour
                    isPending = true,
                    isExisting = true,
                    createdAt = new BsonDateTime(DateTime.Now),
                };

                await accessTokens.InsertOneAsync(tokenDoc.ToBsonDocument());

                await res.WriteAsync(newToken);
            });

            this.Get("/claims/", async (req, res) =>
            {
                string claimToken = req.Cookies["ExperienceCapture-Claim-Token"];
                if (claimToken == null)
                {
                    res.StatusCode = 400;
                    await res.WriteAsync(claimToken);
                    return;
                }

                var claimTokens = db.GetCollection<BsonDocument>("users.tokens.claim");
                var filter = Builders<BsonDocument>.Filter.Eq("hash", PasswordHasher.Hash(claimToken));
                var claimDoc = await claimTokens.Find(filter).FirstOrDefaultAsync();

                if (claimDoc == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                if (!claimDoc["isExisting"].AsBoolean || claimDoc["createdAt"].IsAfter(claimDoc["expirationSeconds"]))
                {
                    res.StatusCode = 404;
                    return;
                }

                if (claimDoc["isPending"].AsBoolean)
                {
                    res.StatusCode = 202;
                    await res.WriteAsync("PENDING");
                    return;
                }

                var update = Builders<BsonDocument>.Update
                    .Set("isExisting", false)
                    .Unset("accessToken");
                await claimTokens.UpdateOneAsync(filter, update);

                await res.WriteAsync(claimDoc["accessToken"].AsString);
            });

            this.Post("/signUp/admin/", async (req, res) =>
            {
                var newAdmin = await req.BindAndValidate<AdminPassword>();
                if (!newAdmin.ValidationResult.IsValid)
                {
                    res.StatusCode = 400;
                    return;
                }

                if (!PasswordHasher.Check(newAdmin.Data.password, PasswordHash))
                {
                    res.StatusCode = 401;
                    return;
                }

                string newToken = Generate.GetRandomToken();
                var signUpTokens = db.GetCollection<BsonDocument>("users.tokens.signUp");

                var tokenDoc = new
                {
                    hash = PasswordHasher.Hash(newToken),
                    expirationSeconds = 3600, // One hour
                    createdAt = new BsonDateTime(DateTime.Now),
                };

                await signUpTokens.InsertOneAsync(tokenDoc.ToBsonDocument());

                await res.WriteAsync(newToken);
            });
        }
    }
}