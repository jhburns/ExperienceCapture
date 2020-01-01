namespace Export.App.Main
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using MongoDB.Bson;
    using MongoDB.Driver;

    public class Export
    {
        private static readonly IMongoDatabase DB = new MongoClient(@"mongodb://db:27017")
            .GetDatabase("ec");

        public static void Main(string[] args)
        {
        }

        private static async void SortSession(string id)
        {
            var sessionCollection = DB.GetCollection<BsonDocument>($"sessions.{id}");

            List<BsonDocument> docs = await sessionCollection
                .Find(Builders<BsonDocument>.Filter.Empty)
                .SortByDescending(d => d["info"]["realtimeSinceStartup"])
                .ToListAsync();

            string docsTotal = "[";
            foreach (BsonDocument d in docs)
            {
                d.Remove("_id");
                docsTotal += d.ToJson() + ",";
            }

            docsTotal = docsTotal.Substring(0, docsTotal.Length - 1);
            docsTotal += "]";

            OutputToFile(docsTotal, id);
        }

        private static void OutputToFile(string content, string id)
        {
            string seperator = Path.DirectorySeparatorChar.ToString();
            string path = $".{seperator}data{seperator}exported{seperator}{id}.sorted.json";

            Console.WriteLine("Outputted to file: " + path);

            System.IO.File.WriteAllText(path, content);
        }
    }
}