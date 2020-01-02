namespace Export.App.Main
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Exporter.App.CustomExceptions;

    using MongoDB.Bson;
    using MongoDB.Driver;

    public class ExportHandler
    {
        private static readonly IMongoDatabase DB = new MongoClient(@"mongodb://db:27017").GetDatabase("ec");

        private static readonly string SessionId = Environment.GetEnvironmentVariable("exporter_session_id")
            ?? throw new EnviromentVarNotSet("The following is unset", "exporter_session_id");

        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            string seperator = Path.DirectorySeparatorChar.ToString();
            await CreateFolder($".{seperator}exported{seperator}");

            await SortSession();

            return;
        }

        private static async Task SortSession()
        {
            var sessionCollection = DB.GetCollection<BsonDocument>($"sessions.{SessionId}");

            List<BsonDocument> docs = await sessionCollection
                .Find(Builders<BsonDocument>.Filter.Empty)
                .SortByDescending(d => d["frameInfo"]["realtimeSinceStartup"])
                .ToListAsync();

            string docsTotal = "[";
            foreach (BsonDocument d in docs)
            {
                d.Remove("_id");
                docsTotal += d.ToJson() + ",";
            }

            docsTotal = docsTotal.Substring(0, docsTotal.Length - 1);
            docsTotal += "]";

            await OutputToFile(docsTotal);
            return;
        }

        private static async Task OutputToFile(string content)
        {
            string seperator = Path.DirectorySeparatorChar.ToString();
            string path = $".{seperator}exported{seperator}{SessionId}.sorted.raw.json";

            using (var sw = new StreamWriter(path))
            {
                await sw.WriteAsync(content);
            }

            return;
        }

        private static async Task CreateFolder(string location)
        {
            await Task.Run(() => Directory.CreateDirectory(location));
        }
    }
}