namespace Carter.App.Route.Users
{
    using System;

    using Carter;

    using Carter.App.Lib.Authentication;
    using Carter.App.Lib.Generate;
    using Carter.App.Lib.Timer;
    using Carter.App.Validation.AccessTokenRequest;
    using Carter.App.Validation.Person;

    using Carter.ModelBinding;
    using Carter.Request;

    using Microsoft.AspNetCore.Http;

    using MongoDB.Bson;
    using MongoDB.Driver;

    public class Users : CarterModule
    {
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

                if (!(await GoogleApi.ValidateUser(newPerson.Data.idToken)))
                {
                    res.StatusCode = 401;
                    return;
                }

                var signUpTokens = db.GetCollection<BsonDocument>("tokens.signUp");

                var filterTokens = Builders<BsonDocument>.Filter.Eq("body", newPerson.Data.signUpToken);
                var existingToken = await signUpTokens.Find(filterTokens).FirstOrDefaultAsync();

                if (existingToken == null)
                {
                    res.StatusCode = 401;
                    return;
                }

                var users = db.GetCollection<BsonDocument>("users");

                var filterUsers = Builders<BsonDocument>.Filter.Eq("id", newPerson.Data.id);
                var existingPerson = await users.Find(filterUsers).FirstOrDefaultAsync();

                if (existingPerson != null)
                {
                    res.StatusCode = 409;
                    return;
                }

                BsonDocument person = new BsonDocument()
                {
                    { "id", newPerson.Data.id },
                    { "fullname", newPerson.Data.fullname },
                    { "firstname", newPerson.Data.firstname },
                    { "lastname", newPerson.Data.lastname },
                    { "email", newPerson.Data.email },
                    { "createdAt", new BsonDateTime(DateTime.Now) },
                };

                await users.InsertOneAsync(person);
                await res.WriteAsync("OK");
            });

            this.Post("/{id:int}/tokens/", async (req, res) =>
            {
                var users = db.GetCollection<BsonDocument>("users");

                int userID = req.RouteValues.As<int>("id");
                var filter = Builders<BsonDocument>.Filter.Eq("id", userID);
                var userDoc = await users.Find(filter).FirstOrDefaultAsync();

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

                if (!(await GoogleApi.ValidateUser(newAccessRequest.Data.idToken)))
                {
                    res.StatusCode = 401;
                    return;
                }

                string newToken = Generate.GetRandomToken(33);
                var accessTokens = db.GetCollection<BsonDocument>("users.tokens.access");

                var tokenObject = new
                {
                    body = newToken,
                    user = userDoc["_id"],
                    expirationSeconds = 259200, // Three days
                    createdAt = new BsonDateTime(DateTime.Now),
                };

                BsonDocument tokenDoc = tokenObject.ToBsonDocument();

                await accessTokens.InsertOneAsync(tokenDoc);

                if (newAccessRequest.Data.claimToken != null)
                {
                    var claimTokens = db.GetCollection<BsonDocument>("users.tokens.claim");

                    var filterClaims = Builders<BsonDocument>.Filter.Eq("body", newAccessRequest.Data.claimToken);
                    var claimDoc = await claimTokens.Find(filterClaims).FirstOrDefaultAsync();

                    if (claimDoc == null)
                    {
                        res.StatusCode = 401;
                        return;
                    }

                    // Don't allow overwriting an access token
                    if (claimDoc["isPending"].AsBoolean && claimDoc["isExisting"].AsBoolean)
                    {
                        var update = Builders<BsonDocument>.Update
                            .Set("isPending", false)
                            .Set("access", tokenDoc["_id"]);
                        await claimTokens.UpdateOneAsync(filterClaims, update);
                    }

                    await res.WriteAsync("OK");
                }
                else
                {
                    await res.WriteAsync(newToken);
                }
            });

            this.Post("/claims/", async (req, res) =>
            {
                string newToken = Generate.GetRandomToken(33);
                var accessTokens = db.GetCollection<BsonDocument>("users.tokens.claim");

                var tokenDoc = new
                {
                    body = newToken,
                    expirationSeconds = 120, // One hour
                    isPending = true,
                    isExisting = true,
                    createdAt = new BsonDateTime(DateTime.Now),
                };

                await accessTokens.InsertOneAsync(tokenDoc.ToBsonDocument());

                await res.WriteAsync(newToken);
            });

            this.Get("/claims/", async (req, res) =>
            {
                string claimToken = req.Headers["ExperienceCapture-Claim-Token"];
                if (claimToken == null)
                {
                    res.StatusCode = 400;
                    return;
                }

                var claimTokens = db.GetCollection<BsonDocument>("users.tokens.claim");
                var filter = Builders<BsonDocument>.Filter.Eq("body", claimToken);
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

                var accessTokens = db.GetCollection<BsonDocument>("users.tokens.access");
                var accessFilter = Builders<BsonDocument>.Filter.Eq("_id", (ObjectId)claimDoc["user"]);
                var accessDoc = await accessTokens.Find(accessFilter).FirstOrDefaultAsync();

                await res.WriteAsync(accessDoc["body"].AsString);
            });

            this.Delete("/claims/", async (req, res) =>
            {
                string claimToken = req.Headers["ExperienceCapture-Claim-Token"];
                if (claimToken == null)
                {
                    res.StatusCode = 400;
                    return;
                }

                var claimTokens = db.GetCollection<BsonDocument>("users.tokens.claim");
                var filter = Builders<BsonDocument>.Filter.Eq("body", claimToken);
                var claimDoc = await claimTokens.Find(filter).FirstOrDefaultAsync();

                if (claimDoc == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                var update = Builders<BsonDocument>.Update.Set("isExisting", false);
                await claimTokens.UpdateOneAsync(filter, update);
                await res.WriteAsync("OK");
            });
        }
    }
}