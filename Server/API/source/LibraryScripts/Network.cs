namespace Carter.App.Lib.Network
{
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Carter.Request;
    using Carter.Response;

    using Microsoft.AspNetCore.Http;

    using MongoDB.Bson;
    using MongoDB.Bson.IO;

    public static class ResponceExtra
    {
        public static async Task FromString(this HttpResponse response, string body = "OK")
        {
            response.ContentType = "text/plain; charset=utf-8";
            await response.WriteAsync(body);
            return;
        }

        public static async Task FromJson(this HttpResponse response, string json)
        {
            response.ContentType = "application/json; charset=utf-8";
            await response.WriteAsync(json);
            return;
        }

        public static async Task FromBson<T>(this HttpResponse response, T doc)
        {
            byte[] clientBson = doc.ToBson();

            using (var mystream = new MemoryStream(clientBson))
            {
                await response.FromStream(mystream, "application/bson");
            }

            return;
        }
    }

    public class JsonQuery
    {
        public static string FulfilEncoding<T>(IQueryCollection query, T document)
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
}