namespace Exporter.App.ExportHandler
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Exporter.App.CustomExceptions;

    using MongoDB.Bson;
    using MongoDB.Driver;

    public class ExportHandler
    {
        private static readonly IMongoDatabase DB = new MongoClient(@"mongodb://db:27017")
            .GetDatabase("ec");

        private static readonly string SessionId = Environment.GetEnvironmentVariable("exporter_session_id")
            ?? throw new EnviromentVarNotSet("The following is unset", "exporter_session_id");

        public static void Start()
        {
            SortSession();
        }

        private static async void SortSession()
        {
            var sessionCollection = DB.GetCollection<BsonDocument>($"sessions.{SessionId}");

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

            OutputToFile(docsTotal);
        }

        private static void OutputToFile(string content)
        {
            string seperator = Path.DirectorySeparatorChar.ToString();
            string path = $".{seperator}data{seperator}exported{seperator}{SessionId}.sorted.json";

            Console.WriteLine("Outputted to file: " + path);

            System.IO.File.WriteAllText(path, content);
        }
    }
}