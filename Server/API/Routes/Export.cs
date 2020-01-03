namespace Carter.App.Route.Export
{
    using System;
    using System.IO;
    using System.Net.Mime;

    using Carter;

    using Carter.App.Lib.Authentication;
    using Carter.App.Lib.DebugExtra;
    using Carter.App.Lib.Generate;
    using Carter.App.Lib.Mongo;
    using Carter.App.Lib.Network;

    using Carter.App.Route.PreSecurity;

    using Carter.Request;
    using Carter.Response;

    using Microsoft.AspNetCore.Http;

    using Minio;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;

    public class Export : CarterModule
    {
        public Export(IMongoDatabase db, MinioClient os)
            : base("/sessions/{id}/export")
        {
            this.Before += PreSecurity.GetSecurityCheck(db);

            this.Post("/", async (req, res) =>
            {
                await res.WriteAsync("NOT IMPLEMENTED");
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