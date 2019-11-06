namespace Nancy.App.Hosting.Kestrel
{
    using Nancy.Extensions;

    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    using Nancy.App.Random;
    using Nancy.App.Session;

    using MongoDB.Driver;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;

    using Network; //

    public class Sessions : NancyModule
    {
        public Sessions(IMongoDatabase db) : base("/sessions/")
        {
            Post("/", args =>
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
                    { "isOpen", true }
                };

                session.InsertOneAsync(sessionDocument);

                byte[] bson = Serial.toBSON(newSession);
                return Response.FromByteArray(bson, "application/bson");
            });

            Post("/{id}", args =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");

                string uniqueID = (string) args.id;
                var filter = Builders<BsonDocument>.Filter.Eq("id", uniqueID);
                var sessionDoc = collection.Find(filter).FirstOrDefault();

                if (sessionDoc == null)
                {
                    return 404;
                }

                if (sessionDoc["isOpen"] == false)
                {
                    return 400;
                }

                string collectionName = $"sessions.{uniqueID}";
                var sessionCollection = db.GetCollection<BsonDocument>("tempSession");
                var document = BsonSerializer.Deserialize<BsonDocument>(Request.Body);
                session.InsertOneAsync(document);

                return "OK";
            });

            Get("/{id}", args =>
            {
                return "OK";
            });

            Delete("/{id}", args =>
            {
                if (!StoreSession.getSessions().Contains(args.id))
                {
                    return 404;
                }

                List<string> ids = StoreSession.getSessions();
                ids.Remove(args.id);
                StoreSession.saveSessions(ids);

                string seperator = Path.DirectorySeparatorChar.ToString();
                string path = $".{seperator}data{seperator}{args.id}.json";

                try
                {
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        var endMessage = new
                        {
                            phase = "Session closed by client.",
                        };

                        sw.WriteLine(JsonConvert.SerializeObject(endMessage));
                        sw.WriteLine("]"); // Close JSON array
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while writing to file: {0}", e);
                    return 500;
                }

                return "OK";
            });
        }
    }
}
