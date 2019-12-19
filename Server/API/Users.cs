namespace Nancy.App.Hosting.Kestrel
{
    using System;

    using MongoDB.Bson;
    using MongoDB.Driver;

    using Nancy.App.Authentication;
    using Nancy.App.Network;
    using Nancy.App.Random;

    public class Users : NancyModule
    {
        public Users(IMongoDatabase db)
            : base("/users/")
        {
            this.Post("/", (args) =>
            {
                var tokens = db.GetCollection<BsonDocument>("tokens");

                // Check if sign-up token is valid, and id token is valid from Google
                // Unless in local dev mode
                // If not, return 401

                // Check if user already exists, if so return 409
                var users = db.GetCollection<BsonDocument>("users");

                // Else return OK
                return "OK";
            });

            this.Post("/{id}/tokens/", (args) =>
            {
                var users = db.GetCollection<BsonDocument>("users");

                // Check if user exists, else return 404

                // Check if jwt is valid from Google, unless in local dev mode
                // If not, return 401
                var tokens = db.GetCollection<BsonDocument>("tokens");

                // Check if is claim token, if so fulfill and return "OK"

                // Else return new API token
                return "API TOKEN";
            });

            this.Post("/claims/", (args) =>
            {
                var claims = db.GetCollection<BsonDocument>("claims");

                // Generate and return new claim token
                return "ClAIM TOKEN";
            });

            this.Get("/claims/", (args) =>
            {
                var claims = db.GetCollection<BsonDocument>("claims");

                // Check if claim exists, else return 404

                // If claim unfilled, return 202

                // Else return API token for claim
                return "API TOKEN";
            });

            this.Delete("/claims/", (args) =>
            {
                var claims = db.GetCollection<BsonDocument>("claims");

                // Check if claim exists, else return 404

                // Else return ok
                return "OK";
            });
        }
    }
}
