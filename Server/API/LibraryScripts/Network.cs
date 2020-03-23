namespace Carter.App.Lib.Network
{
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
            JsonWriterSettings settings = new JsonWriterSettings();
            settings.OutputMode = JsonOutputMode.Strict;

            if (query.As<bool>("ugly"))
            {
                json = document.ToJson(settings);
                json = Regex.Replace(json, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");
                return json;
            }
            else
            {
                settings.Indent = true;
                return document.ToJson(settings);
            }
        }

        public static bool CheckDecoding(IQueryCollection query)
        {
            return query.As<bool>("bson");
        }
    }

    // TODO: Refactor to use extension methods instead
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

    public class BasicResponce
    {
        public static async void Send(HttpResponse response, string body = "OK")
        {
            response.ContentType = "application/text; charset=utf-8";
            await response.WriteAsync(body);
        }
    }
}