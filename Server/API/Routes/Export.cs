namespace Carter.App.Route.Export
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mime;

    using Carter;

    using Carter.App.Lib.CustomExceptions;
    using Carter.App.Lib.Mongo;
    using Carter.App.Lib.Network;
    using Carter.App.Route.PreSecurity;

    using Carter.Request;

    using Docker.DotNet;
    using Docker.DotNet.Models;

    using Minio;

    using MongoDB.Bson;
    using MongoDB.Driver;

    public class Export : CarterModule
    {
        private static readonly string ExporterImageName = Environment.GetEnvironmentVariable("exporter_image_name")
            ?? throw new EnviromentVarNotSet("The following is unset", "exporter_image_name");

        public Export(IMongoDatabase db, MinioClient os, IDockerClient docker)
            : base("/sessions/{id}/export")
        {
            this.Before += PreSecurity.GetSecurityCheck(db);

            this.Post("/", async (req, res) =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");

                string id = req.RouteValues.As<string>("id");
                var filter = Builders<BsonDocument>.Filter
                    .Eq("id", id);
                var sessionDoc = await sessions.Find(filter).FirstOrDefaultAsync();

                if (sessionDoc == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                var exporter = await docker.Containers.CreateContainerAsync(new CreateContainerParameters()
                {
                    Image = ExporterImageName,
                    Cmd = new List<string>() { "dotnet", "Exporter.dll" }, // Don't bother waiting since this API also needs the same resources
                    Tty = true,
                    AttachStdin = true,
                    AttachStdout = true,
                    AttachStderr = true,
                    Env = new List<string>()
                    {
                        "WAIT_HOSTS= db:27017, os:9000",
                        $"exporter_session_id={id}",
                    },
                    HostConfig = new HostConfig()
                    {
                        Memory = 500000000, // 500 MegaBytes
                        CPUPercent = 80, // 80%
                    },
                });

                await docker.Networks.ConnectNetworkAsync("server_ec-network", new NetworkConnectParameters()
                {
                    Container = exporter.ID,
                });

                await docker.Containers.StartContainerAsync(exporter.ID, new ContainerStartParameters());


                var update = Builders<BsonDocument>.Update
                    .Set("isPending", true);

                await sessions.UpdateOneAsync(filter, update);

                BasicResponce.Send(res);
            });

            this.Get("/", async (req, res) =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");

                string id = req.RouteValues.As<string>("id");
                var sessionDoc = await sessions.FindEqAsync("id", id);

                if (sessionDoc == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                if (sessionDoc["isPending"].AsBoolean)
                {
                    res.StatusCode = 202;
                    BasicResponce.Send(res, "PENDING");
                    return;
                }

                // Export job hasn't been created, so return not found
                if (!sessionDoc["isExported"].AsBoolean)
                {
                    res.StatusCode = 404;
                    return;
                }

                string bucketName = "sessions.exported";
                string objectName = $"{id}.exported.zip";
                var about = new ContentDisposition { FileName = objectName };

                res.ContentType = "application/zip";
                res.Headers["Content-Disposition"] = about.ToString();

                await os.GetObjectAsync(bucketName, objectName,
                (stream) =>
                {
                    stream.CopyToAsync(res.Body);
                });
            });
        }
    }
}