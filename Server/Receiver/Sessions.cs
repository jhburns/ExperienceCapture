namespace Nancy.App.Hosting.Kestrel
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;

    using Nancy.App.Random;

    using Network;

    public class Sessions : NancyModule
    {
        public Sessions(IMongoDatabase db)
            : base("/sessions/")
        {
            this.Post("/", args =>
            {
                var session = db.GetCollection<BsonDocument>("sessions");

                string uniqueID = Generate.RandomString(4);

                var newSession = new
                {
                    status = "OK",
                    id = uniqueID,
                };

                var sessionDocument = new BsonDocument
                {
                    { "id", uniqueID },
                    { "isOpen", true },
                };

                session.InsertOneAsync(sessionDocument);

                byte[] bson = Serial.ToBSON(newSession);
                return this.Response.FromByteArray(bson, "application/bson");
            });

            this.Post("/{id}", args =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");

                string uniqueID = (string)args.id;
                var filter = Builders<BsonDocument>.Filter.Eq("id", uniqueID);
                var sessionDoc = sessions.Find(filter).FirstOrDefault();

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
                sessionCollection.InsertOneAsync(document);

                return "OK";
            });

            this.Get("/{id}", args =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");

                string uniqueID = (string)args.id;
                var filter = Builders<BsonDocument>.Filter.Eq("id", uniqueID);
                var sessionDoc = sessions.Find(filter).FirstOrDefault();

                if (sessionDoc == null)
                {
                    return 404;
                }

                if (((bool)this.Request.Query["json"]) == true)
                {
                    return sessionDoc.ToJson();
                }

                byte[] bson = sessionDoc.ToBson();
                return this.Response.FromByteArray(bson, "application/bson");
            });

            this.Delete("/{id}", args =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");

                string uniqueID = (string)args.id;
                var filter = Builders<BsonDocument>.Filter.Eq("id", uniqueID);
                var update = Builders<BsonDocument>.Update.Set("isOpen", false);
                var sessionDoc = sessions.Find(filter).FirstOrDefault();

                if (sessionDoc == null)
                {
                    return 404;
                }

                sessions.UpdateOne(filter, update);

                return "OK";
            });
        }
    }
}
