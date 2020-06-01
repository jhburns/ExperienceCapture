namespace Carter.App.Route.Export
{
    using System.IO;
    using System.Net.Mime;
    using System.Threading;

    using Carter;

    using Carter.App.Export.Main;
    using Carter.App.Hosting;
    using Carter.App.Lib.MinioExtra;
    using Carter.App.Lib.Network;

    using Carter.App.Route.PreSecurity;
    using Carter.App.Route.Sessions;

    using Carter.Request;
    using Carter.Response;

    using MongoDB.Driver;

    public class Export : CarterModule
    {
        public Export(IMongoDatabase db, IMinioClient os)
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

                var exporterConfig = new ExporterConfiguration
                {
                    Mongo = new ServiceConfiguration {
                        ConnectionString = AppConfiguration.Mongo.ConnectionString,
                        Port = AppConfiguration.Mongo.Port,
                    },
                    Minio = new ServiceConfiguration {
                        ConnectionString = AppConfiguration.Minio.ConnectionString,
                        Port = AppConfiguration.Minio.Port,
                    },
                    Id = id,
                };

                export.Start(exporterConfig);

                var update = Builders<SessionSchema>.Update
                    .Set(s => s.IsPending, true);

                await sessions.UpdateOneAsync(filter, update);

                await res.FromString();
            });

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
                    await res.FromString("PENDING");
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
}