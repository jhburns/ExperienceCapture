namespace Carter.App.Lib.Network
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    using Carter.Request;
    using Carter.Response;

    using Microsoft.AspNetCore.Http;

    using MongoDB.Bson;
    using MongoDB.Bson.IO;

    public class JsonQuery
    {
        public static string FulfilEncoding(IQueryCollection query, BsonDocument document)
        {
            if (query.As<bool>("bson"))
            {
                return null;
            }

            string json;

            if (query.As<bool>("ugly"))
            {
                json = document.ToJson();
                json = Regex.Replace(json, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");
                Console.WriteLine(json);
                return json;
            }
            else
            {
                return document.ToJson(new JsonWriterSettings { Indent = true });
            }
        }

        public static BsonDocument FulfilDencoding(IQueryCollection query, string json)
        {
            if (!query.As<bool>("bson"))
            {
                return BsonDocument.Parse(json);
            }

            return null;
        }
    }

    public class BsonResponse
    {
        public static async void FromDoc(HttpResponse response, BsonDocument doc)
        {
            byte[] clientBson = doc.ToBson();

            using (var mystream = new MemoryStream(clientBson))
            {
                await response.FromStream(mystream, "application/bson");
            }
        }
    }

    public class JsonResponce
    {
        public static async void FromString(HttpResponse response, string json)
        {
            response.ContentType = "application/json; charset=utf-8";
            await response.WriteAsync(json);
        }
    }
}