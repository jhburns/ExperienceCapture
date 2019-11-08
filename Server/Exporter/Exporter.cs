namespace Export.App.Main
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MongoDB.Bson;
    using MongoDB.Driver;

    public class Export
    {
        private static IMongoDatabase db;

        public static void Main(string[] args)
        {
            MongoClient client = new MongoClient(@"mongodb://db:27017");
            db = client.GetDatabase("ec");

            Console.WriteLine("Hello ");

            SearchSession("YXN7");
        }

        private static void PromptOption()
        {
        }

        private static async Task<List<BsonDocument>> SearchSession(string id)
        {
            var sessionDocs = db.GetCollection<BsonDocument>($"sessions.{id}");
            List<BsonDocument> docs = await sessionDocs
                .Find(Builders<BsonDocument>.Filter.Empty)
                .SortByDescending(d => d["info"]["realtimeSinceStartup"])
                .ToListAsync();

            return docs;
        }

        private static bool CheckSession(string sessionId)
        {
            var sessions = db.GetCollection<BsonDocument>("sessions");

            var filter = Builders<BsonDocument>.Filter.Eq("id", sessionId);
            var sessionDoc = sessions.Find(filter).FirstOrDefault();

            if (sessionDoc == null)
            {
                return false;
            }

            if (sessionDoc["isOpen"] == false)
            {
                return false;
            }

            return true;
        }

        private static void OutputToFile(string content, string id)
        {
        }
    }
}