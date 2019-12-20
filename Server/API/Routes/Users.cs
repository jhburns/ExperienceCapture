namespace Carter.Route.Users
{
    using Carter;

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
                var tokens = db.GetCollection<BsonDocument>("tokens");

                // Check if sign-up token is valid, and id token is valid from Google
                // Unless in local dev mode
                // If not, return 401

                // Check if user already exists, if so return 409
                var users = db.GetCollection<BsonDocument>("users");

                // Else return OK
                await res.WriteAsync("OK");
            });

            this.Post("/{id}/tokens/", async (req, res) =>
            {
                var users = db.GetCollection<BsonDocument>("users");

                // Check if user exists, else return 404

                // Check if jwt is valid from Google, unless in local dev mode
                // If not, return 401
                var tokens = db.GetCollection<BsonDocument>("tokens");

                // Check if is claim token, if so fulfill and return "OK"

                // Else return new API token
                await res.WriteAsync("API TOKEN");
            });

            this.Post("/claims/", async (req, res) =>
            {
                var claims = db.GetCollection<BsonDocument>("claims");

                // Generate and return new claim token
                await res.WriteAsync("ClAIM TOKEN");
            });

            this.Get("/claims/", async (req, res) =>
            {
                var claims = db.GetCollection<BsonDocument>("claims");

                // Check if claim exists, else return 404

                // If claim unfilled, return 202

                // Else return API token for claim
                await res.WriteAsync("API TOKEN");
            });

            this.Delete("/claims/", async (req, res) =>
            {
                var claims = db.GetCollection<BsonDocument>("claims");

                // Check if claim exists, else return 404

                // Else return ok
                await res.WriteAsync("OK");
            });
        }
    }
}