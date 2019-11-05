namespace Network
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;
    using System.IO;

    using Nancy;

    class Serial
    {
        public static byte[] toBSON(object obj)
        {
            MemoryStream memStream = new MemoryStream();
            using (BsonWriter writer = new BsonWriter(memStream))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, obj);
            }
            return memStream.ToArray();
        }
    }

    // From https://stackoverflow.com/questions/14473510/how-to-make-an-image-handler-in-nancyfx
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

    public static class Extensions
    {
        public static Response FromByteArray(this IResponseFormatter formatter, byte[] body, string contentType = null)
        {
            return new ByteArrayResponse(body, contentType);
        }
    }
}