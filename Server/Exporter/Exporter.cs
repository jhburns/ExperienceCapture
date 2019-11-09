namespace Export.App.Main
{
    using System;
    using MongoDB.Driver;

    public class Export
    {
        public static void Main(string[] args)
        {
            MongoClient client = new MongoClient(@"mongodb://db:27017");
            IMongoDatabase db = client.GetDatabase("ec");

            Console.WriteLine("Hello ");
        }
    }
}