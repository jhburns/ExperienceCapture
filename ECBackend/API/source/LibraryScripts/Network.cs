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

    /// <summary>
    /// HttpResponse extensions.
    /// </summary>
    public static class ResponceExtra
    {
        /// <summary>
        /// Send a string responce.
        /// </summary>
        /// <param name="response">A responce to write to.</param>
        /// <param name="body">The body sent.</param>
        public static async Task FromString(this HttpResponse response, string body = "OK")
        {
            response.ContentType = "text/plain; charset=utf-8";
            await response.WriteAsync(body);
            return;
        }

        /// <summary>
        /// Send a JSON responce.
        /// </summary>
        /// <param name="response">A responce to write to.</param>
        /// <param name="json">The json sent.</param>
        public static async Task FromJson(this HttpResponse response, string json)
        {
            response.ContentType = "application/json; charset=utf-8";
            await response.WriteAsync(json);
            return;
        }

        /// <summary>
        /// Sends a BSON responce.
        /// </summary>
        /// <param name="response">A responce to write to.</param>
        /// <param name="doc">A document to serialize then send.</param>
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

    /// <summary>
    /// Helpers for query string parameters.
    /// </summary>
    public class JsonQuery
    {
        /// <summary>
        /// Determines the proper responce encoding, then applies it.
        /// </summary>
        /// <returns>
        /// Am encoded string, or null if the encoding needs to be BSON.
        /// </returns>
        /// <param name="query">Queries to read.</param>
        /// <param name="document">A document to be encoded.</param>
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

        /// <summary>
        /// Determines the encoding of the request.
        /// </summary>
        /// <returns>
        /// True if there is a request for BSON encoding.
        /// </returns>
        /// <param name="query">Queries to read.</param>
        public static bool CheckDecoding(IQueryCollection query)
        {
            return query.As<bool>("bson");
        }
    }
}