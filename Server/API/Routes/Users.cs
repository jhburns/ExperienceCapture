namespace Carter.App.Route.Users
{
    using System;

    using Carter;

    using Carter.App.Lib.Authentication;
    using Carter.App.Lib.Generate;
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
                var accessTokens = db.GetCollection<BsonDocument>("tokens.access");

                var tokenDoc = new
                {
                    body = newToken,
                    user = new MongoDBRef("users", userDoc),
                    createdAt = new BsonDateTime(DateTime.Now),
                };

                await accessTokens.InsertOneAsync(tokenDoc.ToBsonDocument());

                await res.WriteAsync(newToken);
            });

            this.Post("/claims/", async (req, res) =>
            {
                // Generate and return new claim token
                await res.WriteAsync("ClAIM TOKEN");
            });

            this.Get("/claims/", async (req, res) =>
            {
                // Check if claim exists, else return 404

                // If claim unfilled, return 202

                // Else return API token for claim
                await res.WriteAsync("API TOKEN");
            });

            this.Delete("/claims/", async (req, res) =>
            {
                // Check if claim exists, else return 404

                // Else return ok
                await res.WriteAsync("OK");
            });
        }
    }
}