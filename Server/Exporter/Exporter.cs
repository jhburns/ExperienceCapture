namespace Export.App.Main
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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

            Console.WriteLine("Welcome to the Exporter. (v1.1.0)");
            while (true)
            {
                MatchCommand(PromptOptions());
            }
        }

        private static int PromptOptions()
        {
            Console.WriteLine("--------------------------------------------------------");
            Console.WriteLine("Please select an option.");
            Console.WriteLine("\t1. List all closed sessions.");
            Console.WriteLine("\t2. Download files of closed sessions.");
            Console.WriteLine("\t3. Close this.");
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
                    PrintAllSessions();
                    break;
                case 2:
                    ExportSessions();
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

        private static void PrintAllSessions()
        {
            var sessionCollection = db.GetCollection<BsonDocument>("sessions");
            var filter = Builders<BsonDocument>.Filter.Eq("isOpen", false);

            List<BsonDocument> allSessions = sessionCollection
                .Find(filter)
                .ToList();

            Console.WriteLine("Session IDs");
            foreach (BsonDocument session in allSessions)
            {
                Console.WriteLine(session["id"]);
            }
        }

        private static void ExportSessions()
        {
            Console.WriteLine("Enter Session IDs, separated by a comma:");
            string ids = Console.ReadLine();
            List<string> idList = new List<string>(ids.Split(','));

            idList.ForEach(delegate(string id)
            {
                id.Trim();
            });

            idList.RemoveAll(delegate(string id)
            {
                if (!CheckSession(id))
                {
                    Console.WriteLine($"Error: {id} session doesn't exist or is still open.");
                    return true;
                }

                return false;
            });

            foreach (string id in idList)
            {
                SortSession(id);
            }
        }

        private static async void SortSession(string id)
        {
            var sessionCollection = db.GetCollection<BsonDocument>($"sessions.{id}");

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

        private static bool CheckSession(string sessionId)
        {
            var sessions = db.GetCollection<BsonDocument>("sessions");

            var filter = Builders<BsonDocument>.Filter.Eq("id", sessionId);
            var sessionDoc = sessions.Find(filter).FirstOrDefault();

            if (sessionDoc == null)
            {
                return false;
            }

            if (sessionDoc["isOpen"] == true)
            {
                return false;
            }

            return true;
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