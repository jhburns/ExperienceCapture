/*
- Name: Jonathan Hirokazu Burns
- ID: 2288851
- email: jburns@chapman.edu
- Course: 353-01
- Assignment: Submission #2
- Purpose: Implements the session resource of the API
*/

namespace Nancy.App.Hosting.Kestrel
{
    using MongoDB.Bson;
    using MongoDB.Bson.IO;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;

    using Nancy.App.Random;

    using Network;

    /*
     * Sessions
     * Implements session resource API
     */
    public class Sessions : NancyModule
    {
        /*
         * Sessions
         * Params:
         * - db: the 'ec' database connection
         */
        public Sessions(IMongoDatabase db)
            : base("/sessions/")
        {
            this.Post("/", args =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");

                string uniqueID = Generate.RandomString(4);
                var filter = Builders<BsonDocument>.Filter.Gt("id", uniqueID);
                while (sessions.Find(filter).FirstOrDefault() != null)
                {
                    uniqueID = Generate.RandomString(4);
                    filter = Builders<BsonDocument>.Filter.Gt("id", uniqueID);
                }

                var sessionDoc = new BsonDocument
                {
                    { "id", uniqueID },
                    { "isOpen", true },
                };

                byte[] bson = sessionDoc.ToBson();
                sessions.InsertOneAsync(sessionDoc);

                var clientDoc = new BsonDocument(sessionDoc);
                clientDoc.Remove("_id");
                byte[] clientBson = clientDoc.ToBson();

                return this.Response.FromByteArray(clientBson, "application/bson");
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
                    string json;

                    if (((bool)this.Request.Query["ugly"]) == true)
                    {
                        json = sessionDoc.ToJson();
                    }
                    else
                    {
                        json = sessionDoc.ToJson(new JsonWriterSettings { Indent = true });
                    }

                    return json;
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
