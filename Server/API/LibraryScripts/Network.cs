namespace Network
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    using MongoDB.Bson;
    using MongoDB.Bson.IO;

    using Nancy;

    public static class Extensions
    {
        public static Response FromByteArray(this IResponseFormatter formatter, byte[] body, string contentType = null)
        {
            return new ByteArrayResponse(body, contentType);
        }
    }

    public class ByteArrayResponse : Response
    {
        public ByteArrayResponse(byte[] body, string contentType = null)
        {
            this.ContentType = contentType ?? "application/octet-stream";

            this.Contents = stream =>
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(body);
                }
            };
        }
    }

    public class JsonResponce
    {
        public static string FulfilEncoding(Dictionary<string, DynamicDictionaryValue> query, BsonDocument document)
        {
            string json;

            if (((bool)query["json"]) == true)
            {
                if (((bool)query["ugly"]) == true)
                {
                    json = document.ToJson();
                }
                else
                {
                    json = document.ToJson(new JsonWriterSettings { Indent = true });
                }

                return json;
            }

            return null;
        }
    }
}