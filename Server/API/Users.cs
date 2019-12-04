namespace Nancy.App.Hosting.Kestrel
{
    using System;

    using MongoDB.Bson;
    using MongoDB.Bson.IO;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;

    using Nancy.App.Random;

    using Network;

    public class Users : NancyModule
    {
        public Users(IMongoDatabase db)
            : base("/users/")
        {
            this.Post("/", (args) =>
            {
                var users = db.GetCollection<BsonDocument>("users");

                // Check if sign-up token is valid, and id token is valid from Google
                // Unless in local dev mode
                // If not, return 401

                // Else return OK
                return "OK";
            });

            this.Post("/{id}", (args) =>
            {
                var users = db.GetCollection<BsonDocument>("users");

                // Check if id token is valid from Google, unless in local dev mode
                // If not, return 401
                var tokens = db.GetCollection<BsonDocument>("tokens");

                // Check if claim token was passed,
                // Then fulfill, and return "OK"

                // Else return new API token
                return "API TOKEN";
            });

            this.Post("/claims", (args) =>
            {
                var claims = db.GetCollection<BsonDocument>("claims");

                // Generate and return new claim token
                return "ClAIM TOKEN OBJECT";
            });

            this.Get("/claims/{token}", (args) =>
            {
                var claims = db.GetCollection<BsonDocument>("claims");

                // If claim still unfilled, return 202 -> accepted
                // Get API token for claim
                return "API TOKEN";
            });
        }
    }
}
