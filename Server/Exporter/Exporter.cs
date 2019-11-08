namespace Export.App.Main
{
    using System;
    using System.Collections.Generic;

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
        }

        public static void PromptOption()
        {
        }

        // public static List<BsonValue> SearchSession(string id)
        // {
        // }
        public static void OutputToFile(string content, string id)
        {
        }
    }
}