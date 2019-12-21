namespace Carter.App.Route.Users
{
    using System;

    using Carter;

    using Carter.App.Lib.Authentication;
    using Carter.App.Validation.Person;

    using Carter.ModelBinding;

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
                    { "joinDate", new BsonDateTime(DateTime.Now) },
                };

                await users.InsertOneAsync(person);
                await res.WriteAsync("OK");
            });

            this.Post("/{id}/tokens/", async (req, res) =>
            {
                // Check if user exists, else return 404

                // Check if jwt is valid from Google, unless in local dev mode
                // If not, return 401
                // Check if is claim token, if so fulfill and return "OK"

                // Else return new API token
                await res.WriteAsync("API TOKEN");
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