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
                        sw.WriteLine("{");
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

                Console.WriteLine(ids.Count);

                return JsonConvert.SerializeObject(newSession);
            });

            Post("/session/{id}", args =>
            {
                if (!StoreSession.getSessions().Contains(args.id))
                {
                    return 404;
                }

                string chunk = Request.Body.AsString();

                string seperator = Path.DirectorySeparatorChar.ToString();
                string path = $".{seperator}data{seperator}{args.id}.json";

                try
                {
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.Write(chunk);
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