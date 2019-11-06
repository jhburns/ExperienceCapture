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
}