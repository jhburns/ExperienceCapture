using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;


using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Processor
{
    class Process
    {
        static void Main(string[] args)
        {
            string filename = Environment.GetEnvironmentVariable("filename");

            string seperator = Path.DirectorySeparatorChar.ToString();
            string path = $".{seperator}data{seperator}{filename}";
            string textData = System.IO.File.ReadAllText(path);

            dynamic data = JsonConvert.DeserializeObject<dynamic>(textData);

            JArray sortable = new JArray();
            JArray unsortable = new JArray();

            foreach (JObject o in data)
            {
                dynamic info = o["info"];
                if (info != null)
                {
                    sortable.Add(o);
                }
                else
                {
                    unsortable.Add(o);
                }
            }

            List<List<float>> timestamps = new List<List<float>>();

            for (int i = 0; i < sortable.Count; i++)
            {
                List<float> stamp = new List<float>()
                {
                    i,
                    (float) (sortable[i]["info"]["timestamp"])
                };

                timestamps.Add(stamp);
            }

            List<List<float>> sortedStamps = timestamps.OrderBy(i => i[1]).ToList();

            JArray sorted = new JArray();
            foreach (List<float> l in sortedStamps)
            {
                sorted.Add(sortable[(int) l[0]]);
            }

            string outPath = $".{seperator}data{seperator}processed{seperator}processed.{filename}";

            System.IO.File.WriteAllText(outPath, JsonConvert.SerializeObject(sorted, Formatting.Indented));

            Console.WriteLine("Done processing file.");
        }

    }
}