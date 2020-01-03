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

        private static readonly string Seperator = Path.DirectorySeparatorChar.ToString();

        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            await CreateFolder($".{Seperator}exported{Seperator}");

            await ExportSession();

            // System.Threading.Thread.Sleep(100000000); // To make it so the program doesn't exist immediately

            return;
        }

        private static async Task ExportSession()
        {
            List<BsonDocument> sessionSorted = await SortSession();
            
            await ToJson(sessionSorted);

            return;
        }

        private static async Task<List<BsonDocument>> SortSession()
        {
            var sessionCollection = DB.GetCollection<BsonDocument>($"sessions.{SessionId}");

            var filter = Builders<BsonDocument>.Filter.Empty;
            var sorter = Builders<BsonDocument>.Sort
                .Ascending(d => d["frameInfo"]["realtimeSinceStartup"]);
            var projection = Builders<BsonDocument>.Projection
                .Exclude("_id");

            return await sessionCollection
                .Find(filter)
                .Sort(sorter)
                .Project(projection)
                .ToListAsync();
        }

        private static async Task ToJson(List<BsonDocument> sessionDocs) {
            string docsTotal = "[";
            foreach (BsonDocument d in sessionDocs)
            {
                docsTotal += d.ToJson() + ",";
            }

            docsTotal = docsTotal.Substring(0, docsTotal.Length - 1);
            docsTotal += "]";

            string filename = $"{SessionId}.sorted.raw.json";
            await OutputToFile(docsTotal, filename);
            return;
        }

        private static async Task OutputToFile(string content, string filename)
        {
            string path = $".{Seperator}exported{Seperator}{filename}";

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