namespace Carter.App.Route.Export
{
    using System.IO;
    using System.Net.Mime;
    using System.Threading;

    using Carter;

    using Carter.App.Export.Hosting;
    using Carter.App.Export.Main;
    using Carter.App.Hosting;

    using Carter.App.Lib.ExporterExtra;
    using Carter.App.Lib.MinioExtra;
    using Carter.App.Lib.Network;
    using Carter.App.Lib.Repository;
    using Carter.App.Lib.Timer;

    using Carter.App.Route.PreSecurity;
    using Carter.App.Route.Sessions;
    using Carter.App.Route.UsersAndAuthentication;

    using Carter.Request;
    using Carter.Response;

    using MongoDB.Driver;

    using static Microsoft.AspNetCore.Http.StatusCodes;

    /// <summary>
    /// Export routes.
    /// </summary>
    public class Export : CarterModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Export"/> class.
        /// </summary>
        public Export(
            IRepository<AccessTokenSchema> accessRepo,
            IRepository<SessionSchema> sessionRepo,
            IThreadExtra threader,
            IMinioClient os,
            IDateExtra date)
            : base("/sessions/{id}/export")
        {
            this.Before += PreSecurity.CheckAccess(accessRepo, date);

            this.Post("/", async (req, res) =>
            {
                string id = req.RouteValues.As<string>("id");

                var sessionDoc = await sessionRepo
                    .FindById(id);

                if (sessionDoc == null)
                {
                    res.StatusCode = Status404NotFound;
                    return;
                }

                var exporterConfig = new ExporterConfiguration
                {
                    Mongo = new ConnectionConfiguration
                    {
                        ConnectionString = AppConfiguration.Mongo.ConnectionString,
                        Port = AppConfiguration.Mongo.Port,
                    },
                    Minio = new ConnectionConfiguration
                    {
                        ConnectionString = AppConfiguration.Minio.ConnectionString,
                        Port = AppConfiguration.Minio.Port,
                    },
                    Id = id,
                };

                threader.Run(ExportHandler.Entry, exporterConfig);

                var filter = Builders<SessionSchema>.Filter
                    .Where(s => s.Id == id);

                var update = Builders<SessionSchema>.Update
                    .Set(s => s.ExportState, ExportOptions.Pending);

                await sessionRepo.Update(filter, update);

                await res.FromString();
            });

            this.Get("/", async (req, res) =>
            {
                string id = req.RouteValues.As<string>("id");
                var sessionDoc = await sessionRepo.FindById(id);

                if (sessionDoc == null)
                {
                    res.StatusCode = Status404NotFound;
                    return;
                }

                if (sessionDoc.ExportState == ExportOptions.Pending)
                {
                    res.StatusCode = Status202Accepted;
                    await res.FromString("PENDING");
                    return;
                }

                // Export job hasn't been created, so return not found
                if (sessionDoc.ExportState == ExportOptions.NotStarted)
                {
                    res.StatusCode = Status404NotFound;
                    return;
                }

                // Not sure what to do about an error
                if (sessionDoc.ExportState == ExportOptions.Error)
                {
                    res.StatusCode = Status500InternalServerError;
                    return;
                }

                string bucketName = "sessions.exported";
                string objectName = $"{id}_session_exported.zip";
                byte[] body = await os.GetBytesAsync(bucketName, objectName);

                var about = new ContentDisposition { FileName = objectName };
                using (var stream = new MemoryStream(body))
                {
                    res.ContentLength = body.Length;
                    await res.FromStream(stream, "application/zip", about);
                }
            });
        }
    }

    /// <summary>
    /// Implementation of IThreadExtra.
    /// </summary>
    public sealed class ExportThreader : IThreadExtra
    {
        /// <inheritdoc />
        public void Run(ParameterizedThreadStart method, object parameter = null)
        {
            var export = new Thread(ExportHandler.Entry);
            export.Start(parameter);
        }
    }
}