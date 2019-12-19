namespace Network
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;

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

    public class JsonQuery
    {
        public static string FulfilEncoding(DynamicDictionary query, BsonDocument document)
        {
            string json;

            if (((bool)query["json"]) == true)
            {
                if (((bool)query["ugly"]) == true)
                {
                    json = document.ToJson();
                    json = Regex.Replace(json, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");
                }
                else
                {
                    json = document.ToJson(new JsonWriterSettings { Indent = true });
                }

                return json;
            }

            return null;
        }

        public static BsonDocument FulfilDencoding(DynamicDictionary query, string json)
        {
            if (((bool)query["json"]) == true)
            {
                return BsonDocument.Parse(json);
            }

            return null;
        }
    }
}