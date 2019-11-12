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

            MatchCommand(PromptOptions());
        }

        private static int PromptOptions()
        {
            Console.WriteLine("Welcome to the Exporter, please input an option:");
            Console.WriteLine("1. List all sessions.");
            Console.WriteLine("2. Download files of sessions.");
            Console.WriteLine("3. Close.");
            Console.WriteLine("Option (1-3):");

            int commandValue;
            try
            {
                commandValue = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Please only enter an number");
                Console.WriteLine(string.Empty);
                return PromptOptions();
            }

            return commandValue;
        }

        private static void MatchCommand(int commandValue)
        {
            Console.WriteLine(string.Empty);

            switch (commandValue)
            {
                case 1:
                    Console.WriteLine("Case 1");
                    break;
                case 2:
                    Console.WriteLine("Case 2");
                    break;
                case 3:
                    Console.WriteLine("Closing...");
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Please input a number in range 1-3.");
                    MatchCommand(PromptOptions());
                    break;
            }
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