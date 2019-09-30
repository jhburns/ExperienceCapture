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

    public class Routes : NancyModule
    {
        public Routes(IAppConfiguration appConfig)
        {
            Get("/", args => "The receiving server is running.");

            Get("/health", args => "OK");

            Get("/session", args =>
            {
                string uniqueID = Generate.RandomString(8);

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
                        sw.WriteLine("[");
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

            Post("/session/{id}", args =>
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

                Console.WriteLine(chunk);

                string seperator = Path.DirectorySeparatorChar.ToString();
                string path = $".{seperator}data{seperator}{args.id}.json";

                try
                {
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.Write(chunk + ",\n"); // Comma used to string object together
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while writing to file: {0}", e);
                    return 500;
                }
          

                return "OK";
            });

            Delete("/session/{id}", args =>
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
                        sw.WriteLine("]"); // Close array
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