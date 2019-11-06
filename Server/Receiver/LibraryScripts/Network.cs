namespace Network
{
    using System.Collections;
    using System.IO;

    using MongoDB.Bson;

    using Nancy;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;

    // code for FromByteArray from here:
    // https://stackoverflow.com/questions/14473510/how-to-make-an-image-handler-in-nancyfx/28623873
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

    // ^end
    public class Serial
    {
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