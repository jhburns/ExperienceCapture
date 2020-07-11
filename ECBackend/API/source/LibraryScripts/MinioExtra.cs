namespace Carter.App.Lib.MinioExtra
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Minio;
    using Minio.DataModel;

    /// <summary>
    /// A loose copy of Minio's interfaces, see: https://github.com/minio/minio-dotnet.
    /// </summary>
    public interface IMinioClient
    {
        /// <summary>
        /// Returns true if the specified bucketName exists, otherwise returns false.
        /// </summary>
        /// <param name="bucketName">Bucket to test existence of.</param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
        /// <returns>Task that returns true if exists and user has access.</returns>
        Task<bool> BucketExistsAsync(string bucketName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get an object. The object will be streamed to the callback given by the user.
        /// </summary>
        /// <returns>
        /// A Task.
        /// </returns>
        /// <param name="bucketName">Bucket to retrieve object from.</param>
        /// <param name="objectName">Name of object to retrieve.</param>
        /// <param name="callback">A stream will be passed to the callback.</param>
        /// <param name="sse">Optional ECBackend-side encryption option. Defaults to null.</param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
        Task GetObjectAsync(string bucketName, string objectName, Action<Stream> callback, ServerSideEncryption sse = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Create a private bucket with the given name.
        /// </summary>
        /// <returns>
        /// A Task.
        /// </returns>
        /// <param name="bucketName">Name of the new bucket.</param>
        /// <param name="location">Region.</param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
        Task MakeBucketAsync(string bucketName, string location = "us-east-1", CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates an object from file.
        /// </summary>
        /// <returns>
        /// A Task.
        /// </returns>
        /// <param name="bucketName">Bucket to create object in.</param>
        /// <param name="objectName">Key of the new object.</param>
        /// <param name="filePath">Path of file to upload.</param>
        /// <param name="contentType">Content type of the new object, null defaults to "application/octet-stream".</param>
        /// <param name="metaData">Optional Object metadata to be stored. Defaults to null.</param>
        /// <param name="sse">Optional ECBackend-side encryption option. Defaults to null.</param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
        Task PutObjectAsync(string bucketName, string objectName, string filePath, string contentType = null, Dictionary<string, string> metaData = null, ServerSideEncryption sse = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets an object and converts it to a byte array.
        /// </summary>
        /// <returns>
        /// The object in byte form.
        /// </returns>
        /// <param name="bucketName">Bucket to get an object from.</param>
        /// <param name="objectName">Key of the object.</param>
        Task<byte[]> GetBytesAsync(string bucketName, string objectName);
    }

    /// <summary>
    /// Merges together the custom interface and the client library's class.
    /// </summary>
    public class MinioClientExtra : MinioClient, IMinioClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MinioClientExtra"/> class. Passes through to the base constructor.
        /// </summary>
        /// <param name="endpoint">Location of the server, supports HTTP and HTTPS.</param>
        /// <param name="accessKey">Access Key for authenticated requests (Optional, can be omitted for anonymous requests).</param>
        /// <param name="secretKey">Secret Key for authenticated requests (Optional, can be omitted for anonymous requests).</param>
        /// <param name="region">Optional custom region.</param>
        /// <param name="sessionToken">Optional session token.</param>
        /// <returns>Client initialized with user credentials.</returns>
        public MinioClientExtra(string endpoint, string accessKey = "", string secretKey = "", string region = "", string sessionToken = "")
            : base(endpoint, accessKey, secretKey, region, sessionToken)
        {
        }

        // Minio client expects the CopyTo to be both sync and async,
        // So it is wrapped in a function call.

        /// <inheritdoc cref="IMinioClient" />
        public async Task<byte[]> GetBytesAsync(string bucketName, string objectName)
        {
            byte[] objectBytes = null;

            try
            {
                await this.GetObjectAsync(bucketName, objectName, (stream) =>
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