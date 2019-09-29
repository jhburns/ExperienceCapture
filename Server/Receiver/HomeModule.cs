namespace Nancy.App.Hosting.Kestrel
{
    using ModelBinding;
    using System;
    using Nancy.Extensions;

    using System.IO;
    using System.Text;

    using Newtonsoft.Json;

    public class HomeModule : NancyModule
    {
        public HomeModule(IAppConfiguration appConfig)
        {
            Get("/", args => "The receiving server is running.");

            Get("/session", args =>
            {
                string uniqueID = "4321";

                var newSession = new
                {
                    status = "OK",
                    id = uniqueID,
                };

                string seperator = Path.DirectorySeparatorChar.ToString();
                string path = $".{seperator}data{seperator}{uniqueID}";

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

                return JsonConvert.SerializeObject(newSession);
            });

            Post("/save", args =>
            {
                string saveData = Request.Body.AsString();

                string seperator = Path.DirectorySeparatorChar.ToString();
                string path = $".{seperator}data{seperator}alice.json";

                Console.WriteLine("got");

                try
                {
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.Write(saveData);
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