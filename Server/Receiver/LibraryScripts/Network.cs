/*
- Name: Jonathan Hirokazu Burns
- ID: 2288851
- email: jburns@chapman.edu
- Course: 353-01
- Assignment: Submission #1
- Purpose: To be a general purpose networking library
*/

namespace Network
{
    using System.Collections;
    using System.IO;

    using MongoDB.Bson;

    using Nancy;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;

    /*
     * Extensions
     * See Nancy documentation
     */
    public static class Extensions
    {
        /*
         * FromByteArray
         * Params:
         * - formatter: see Nancy documentation
         * - body: the content of the responce
         * - contentType: the HTTP content type flag
         * Returns: a Response object, see Nancy documentation
         */
        public static Response FromByteArray(this IResponseFormatter formatter, byte[] body, string contentType = null)
        {
            return new ByteArrayResponse(body, contentType);
        }
    }

    /*
     * ByteArrayResponse
     * Custom response type
     */
    public class ByteArrayResponse : Response
    {
        /*
         * FromByteArray
         * Params:
         * - body: the content of the responce
         * - contentType: the HTTP content type flag
         */
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

    /*
     * Serial
     * Catch-all for serialization needs 
     */
    public class Serial
    {
        /*
         * ToBSON
         * Params:
         * - obj: the object to serialize
         * Returns: byte array of the serialize object
         */
        public static byte[] ToBSON(object obj)
        {
            MemoryStream memStream = new MemoryStream();
            using (BsonDataWriter writer = new BsonDataWriter(memStream))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, obj);
            }

            return memStream.ToArray();
        }

        /*
         * FromBSON
         * Params:
         * - memStream: data to be deserialized
         * Returns: Type T of resulting object
         */
        public static T FromBSON<T>(MemoryStream memStream)
        {
            T obj;
            using (BsonDataReader reader = new BsonDataReader(memStream))
            {
                JsonSerializer serializer = new JsonSerializer();

                obj = serializer.Deserialize<T>(reader);
            }

            return obj;
        }
    }
}
 