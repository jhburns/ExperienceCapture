namespace Nancy.App.Hosting.Kestrel
{
    using ModelBinding;
    using Nancy.Extensions;

    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    using Nancy.App.Random;
    using Nancy.App.Session;

    using MongoDB.Driver;

    public class Sessions : NancyModule
    {
        public Sessions(IMongoDatabase db) : base("/sessions/")
        {
            Post("/", args =>
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

                List<string> ids = StoreSession.getSessions();
                ids.Add(uniqueID);
                StoreSession.saveSessions(ids);

                return JsonConvert.SerializeObject(newSession);
            });

            Post("/{id}", args =>
            {
                if (!StoreSession.getSessions().Contains(args.id))
                {
                    return 404;
                }

                string chunk = Request.Body.AsString();

                if (chunk == "")
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