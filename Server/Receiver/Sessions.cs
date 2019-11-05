namespace Nancy.App.Hosting.Kestrel
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using MongoDB.Bson;
    using MongoDB.Driver;

    using Nancy.App.Random;
    using Nancy.App.Session;
    using Nancy.Extensions;

    using Network;

    using Newtonsoft.Json;

    public class Sessions : NancyModule
    {
        public Sessions(IMongoDatabase db)
            : base("/sessions/")
        {
            this.Post("/", args =>
            {
                string uniqueID = Generate.RandomString(4);

                var newSession = new
                {
                    status = "OK",
                    id = uniqueID,
                };

                string seperator = Path.DirectorySeparatorChar.ToString();
                string path = $".{seperator}data{seperator}{uniqueID}.json";

                try
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine("["); // Open JSON array
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while creating file: {0}", e);
                    return 500;
                }

                List<string> ids = StoreSession.GetSessions();
                ids.Add(uniqueID);
                StoreSession.SaveSessions(ids);

                byte[] response = Serial.ToBSON(newSession);

                return this.Response.FromByteArray(response, "application/bson");
            });

            this.Post("/{id}", args =>
            {
                var session = db.GetCollection<BsonDocument>("tempSession");
                string chunk = this.Request.Body.AsString();
                MongoDB.Bson.BsonDocument document
                = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(chunk);
                session.InsertOneAsync(document);

                if (!StoreSession.GetSessions().Contains(args.id))
                {
                    return 404;
                }

                // string chunk = Request.Body.AsString();
                if (chunk == string.Empty)
                {
                    return 400;
                }

                string seperator = Path.DirectorySeparatorChar.ToString();
                string path = $".{seperator}data{seperator}{args.id}.json";

                try
                {
                    File.AppendAllText(path, chunk + "," + Environment.NewLine);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while writing to file: {0}", e);
                    return 500;
                }

                return "OK";
            });

            this.Get("/{id}", args =>
            {
                return "OK";
            });

            this.Delete("/{id}", args =>
            {
                if (!StoreSession.GetSessions().Contains(args.id))
                {
                    return 404;
                }

                List<string> ids = StoreSession.GetSessions();
                ids.Remove(args.id);
                StoreSession.SaveSessions(ids);

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
