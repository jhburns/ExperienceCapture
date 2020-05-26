namespace Carter.App.Lib.MinioExtra
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Minio.DataModel;

    public interface IMinioClient
    {
        Task GetObjectAsync(string buckName, string objectName, Action<Stream> callback, ServerSideEncryption sse = null, CancellationToken cancellationToken = default(CancellationToken));
    }

    // Minio client expects the CopyTo to be both sync and async,
    // So it is wrapped in a function call
    internal static class MinioExtra
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