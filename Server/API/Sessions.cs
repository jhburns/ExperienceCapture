namespace Nancy.App.Hosting.Kestrel
{
    using System;

    using MongoDB.Bson;
    using MongoDB.Bson.IO;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;

    using Nancy.App.Random;

    using Network;

    public class Sessions : NancyModule
    {
        public Sessions(IMongoDatabase db)
            : base("/sessions/")
        {
            this.Post("/", async (args) =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");

                string uniqueID = Generate.RandomString(4);
                var filter = Builders<BsonDocument>.Filter.Eq("id", uniqueID);
                while ((await sessions.Find(filter).FirstOrDefaultAsync()) != null)
                {
                    Console.WriteLine();
                    uniqueID = Generate.RandomString(4);
                    filter = Builders<BsonDocument>.Filter.Eq("id", uniqueID);
                }

                var sessionDoc = new BsonDocument
                {
                    { "id", uniqueID },
                    { "isOpen", true },
                };

                byte[] bson = sessionDoc.ToBson();
                _ = sessions.InsertOneAsync(sessionDoc);

                var clientDoc = new BsonDocument(sessionDoc);
                clientDoc.Remove("_id");
                byte[] clientBson = clientDoc.ToBson();

                return this.Response.FromByteArray(clientBson, "application/bson");
            });

            this.Post("/{id}", async (args) =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");

                string uniqueID = (string)args.id;
                var filter = Builders<BsonDocument>.Filter.Eq("id", uniqueID);
                var sessionDoc = await sessions.Find(filter).FirstOrDefaultAsync();

                if (sessionDoc == null)
                {
                    return 404;
                }

                if (sessionDoc["isOpen"] == false)
                {
                    return 400;
                }

                string collectionName = $"sessions.{uniqueID}";
                var sessionCollection = db.GetCollection<BsonDocument>(collectionName);
                var document = BsonSerializer.Deserialize<BsonDocument>(this.Request.Body);
                _ = sessionCollection.InsertOneAsync(document);

                return "OK";
            });

            this.Get("/{id}", async (args) =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");

                string uniqueID = (string)args.id;
                var filter = Builders<BsonDocument>.Filter.Eq("id", uniqueID);
                var sessionDoc = await sessions.Find(filter).FirstOrDefaultAsync();

                if (sessionDoc == null)
                {
                    return 404;
                }

                string json = JsonResponce.FulfilEncoding(this.Request.Query, sessionDoc);
                if (json != null)
                {
                    return json;
                }

                byte[] bson = sessionDoc.ToBson();
                return this.Response.FromByteArray(bson, "application/bson");
            });

            this.Delete("/{id}", async (args) =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");

                string uniqueID = (string)args.id;
                var filter = Builders<BsonDocument>.Filter.Eq("id", uniqueID);
                var update = Builders<BsonDocument>.Update.Set("isOpen", false);
                var sessionDoc = await sessions.Find(filter).FirstOrDefaultAsync();

                if (sessionDoc == null)
                {
                    return 404;
                }

                _ = sessions.UpdateOneAsync(filter, update);

                return "OK";
            });
        }
    }
}
