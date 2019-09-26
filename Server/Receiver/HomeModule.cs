namespace Nancy.App.Hosting.Kestrel
{
    using ModelBinding;
    using System;
    using Nancy.Extensions;

    using System.IO;
    using System.Text;


    public class HomeModule : NancyModule
    {
        public HomeModule(IAppConfiguration appConfig)
        {
            Get("/", args => "OK");

            Post("/", args =>
            {
                var person = this.BindAndValidate<Person>();

                if (!this.ModelValidationResult.IsValid)
                {
                    return 422;
                }

                return person;
            });

            Post("/save", args =>
            {
                string saveData = Request.Body.AsString();

                string seperator = Path.DirectorySeparatorChar.ToString();
                string path = $".{seperator}data{seperator}alice.json";

                Console.WriteLine("got");

                try
                {
                    // Create a file first it doesn't exist 
                    if (!File.Exists(path))
                    {
                        using (StreamWriter sw = File.CreateText(path))
                        {
                            sw.WriteLine("{");
                            sw.Write(saveData);
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.Write(saveData);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while writing to file: {0}", e);
                }
          

                return "OK";
            });
        }
    }
}