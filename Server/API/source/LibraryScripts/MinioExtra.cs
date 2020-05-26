namespace Carter.App.Lib.MinioExtra
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Minio;
    using Minio.DataModel;

    public interface IMinioClient
    {
        /// <summary>
        /// Get an object. The object will be streamed to the callback given by the user.
        /// </summary>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectName">Name of object to retrieve</param>
        /// <param name="callback">A stream will be passed to the callback</param>
        /// <param name="sse">Optional Server-side encryption option. Defaults to null.</param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the operation</param>
        Task GetObjectAsync(string bucketName, string objectName, Action<Stream> callback, ServerSideEncryption sse = null, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class MinioClientExtra : MinioClient, IMinioClient
    {
        /// <summary>
        /// Creates and returns an Cloud Storage client
        /// </summary>
        /// <param name="endpoint">Location of the server, supports HTTP and HTTPS</param>
        /// <param name="accessKey">Access Key for authenticated requests (Optional, can be omitted for anonymous requests)</param>
        /// <param name="secretKey">Secret Key for authenticated requests (Optional, can be omitted for anonymous requests)</param>
        /// <param name="region">Optional custom region</param>
        /// <param name="sessionToken">Optional session token</param>
        /// <returns>Client initialized with user credentials</returns>
        public MinioClientExtra(string endpoint, string accessKey = "", string secretKey = "", string region = "", string sessionToken = "")
            : base(endpoint, accessKey, secretKey, region, sessionToken)
        {
        }
    }

    // Minio client expects the CopyTo to be both sync and async,
    // So it is wrapped in a function call
    internal static class MinioExtensions
    {
        public static async Task<byte[]> GetBytesAsync(this IMinioClient os, string bucketName, string objectName)
        {
            byte[] objectBytes = null;

            try
            {
                await os.GetObjectAsync(bucketName, objectName, (stream) =>
                {
                    using (var outStream = new MemoryStream())
                    {
                        stream.CopyTo(outStream);
                        objectBytes = outStream.ToArray();
                    }
                });

                return objectBytes;
            }
            catch (Exception e)
            {
                // Should never be thrown as object state is checked from Mongo
                throw new Exception($"GetBytesAsync Error. ObjectId: {objectName}, Bucket: {bucketName}", e);
            }
        }
    }
}