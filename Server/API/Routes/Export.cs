namespace Carter.App.Route.Export
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Mime;
    using System.Threading.Tasks;

    using Carter;

    using Carter.App.Lib.CustomExceptions;
    using Carter.App.Lib.Mongo;
    using Carter.App.Lib.Network;

    using Carter.App.Route.PreSecurity;
    using Carter.App.Route.Sessions;

    using Carter.Request;
    using Carter.Response;

    using Docker.DotNet;
    using Docker.DotNet.Models;

    using Minio;

    using MongoDB.Bson;
    using MongoDB.Driver;

    public class Export : CarterModule
    {
        private static readonly string ExporterImageName = Environment.GetEnvironmentVariable("exporter_image_name")
            ?? throw new EnviromentVarNotSet("The following is unset", "exporter_image_name");

        private static readonly string NetworkName = Environment.GetEnvironmentVariable("ec_network_name")
            ?? throw new EnviromentVarNotSet("The following is unset", "ec_network_name");

        public Export(IMongoDatabase db, MinioClient os, IDockerClient docker)
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

                // Warning: doing all of this is slow
                // About 2 seconds
                var exporter = await docker.Containers.CreateContainerAsync(new CreateContainerParameters()
                {
                    Image = ExporterImageName,
                    #pragma warning disable SA1515
                    // Don't bother using wait-for since this API also needs the same resources
                    Cmd = new List<string>() { "dotnet", "Exporter.dll" },
                    #pragma warning restore SA1515
                    Tty = true,
                    AttachStdin = true,
                    AttachStdout = true,
                    AttachStderr = true,
                    Env = new List<string>()
                    {
                        $"exporter_session_id={id}",
                    },
                    HostConfig = new HostConfig()
                    {
                        Memory = 500000000, // ~0.5 Gigabytes
                    },
                });

                await docker.Networks.ConnectNetworkAsync(NetworkName, new NetworkConnectParameters()
                {
                    Container = exporter.ID,
                });

                await docker.Containers.StartContainerAsync(exporter.ID, new ContainerStartParameters());

                var update = Builders<SessionSchema>.Update
                    .Set(s => s.IsPending, true);

                await sessions.UpdateOneAsync(filter, update);

                BasicResponce.Send(res);
            });

            // TODO: Evaluate whether this endpoint is useful or not
            // The normal GET /session/{id}/ endpoint contains the same data
            this.Get("/", async (req, res) =>
            {
                var sessions = db.GetCollection<SessionSchema>("sessions");

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