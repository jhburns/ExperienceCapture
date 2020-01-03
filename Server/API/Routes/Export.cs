namespace Carter.App.Route.Sessions
{
    using System;
    using System.IO;

    using Carter;

    using Carter.App.Lib.Authentication;
    using Carter.App.Lib.DebugExtra;
    using Carter.App.Lib.Generate;
    using Carter.App.Lib.Mongo;
    using Carter.App.Lib.Network;

    using Carter.App.Route.PreSecurity;

    using Carter.Request;

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

            /*
            this.Post("/", async (req, res) =>
            {

            });

            this.Get("/", async (req, res) =>
            {

            });
            */
        }
    }
}