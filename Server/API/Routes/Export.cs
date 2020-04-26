namespace Carter.App.Route.Export
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;

    using Carter;

    using Carter.App.Export.Main;

    using Carter.App.Lib.Network;

    using Carter.App.Route.PreSecurity;
    using Carter.App.Route.Sessions;

    using Carter.Request;
    using Carter.Response;

    using Docker.DotNet;

    using Minio;

    using MongoDB.Driver;

    public class Export : CarterModule
    {
        public Export(IMongoDatabase db, MinioClient os)
            : base("/sessions/{id}/export")
        {
            this.Before += PreSecurity.GetSecurityCheck(db);

            this.Post("/", async (req, res) =>
            {
                var sessions = db.GetCollection<SessionSchema>(SessionSchema.CollectionName);

                string id = req.RouteValues.As<string>("id");
                var filter = Builders<SessionSchema>.Filter
                    .Where(s => s.Id == id);

                var sessionDoc = await sessions.Find(filter).FirstOrDefaultAsync();

                if (sessionDoc == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                var export = new Thread(ExportHandler.Entry);
                export.Start(id);

                var update = Builders<SessionSchema>.Update
                    .Set(s => s.IsPending, true);

                await sessions.UpdateOneAsync(filter, update);

                BasicResponce.Send(res);
            });

            // TODO: Evaluate whether this endpoint is useful or not
            // The normal GET /session/{id}/ endpoint contains the same data
            this.Get("/", async (req, res) =>
            {
                var sessions = db.GetCollection<SessionSchema>(SessionSchema.CollectionName);

                string id = req.RouteValues.As<string>("id");
                var sessionDoc = await sessions.Find(
                    Builders<SessionSchema>
                        .Filter
                        .Where(s => s.Id == id))
                        .FirstOrDefaultAsync();

                if (sessionDoc == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                if (sessionDoc.IsPending)
                {
                    res.StatusCode = 202;
                    BasicResponce.Send(res, "PENDING");
                    return;
                }

                // Export job hasn't been created, so return not found
                if (!sessionDoc.IsExported)
                {
                    res.StatusCode = 404;
                    return;
                }

                string bucketName = "sessions.exported";
                string objectName = $"{id}_session_exported.zip";
                byte[] body = await os.GetBytesAsync(bucketName, objectName);

                var about = new ContentDisposition { FileName = objectName };
                using (var stream = new MemoryStream(body))
                {
                    await res.FromStream(stream, "application/zip", about);
                }
            });
        }
    }

    // Minio client expects the CopyTo to be both sync and async,
    // So it is wrapped in a function call
    internal static class MinioExtra
    {
        public static async Task<byte[]> GetBytesAsync(this MinioClient os, string bucketName, string objectName)
        {
            byte[] objectBytes = null;

            try
            {
                await os.GetObjectAsync(bucketName, objectName,
                (stream) =>
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