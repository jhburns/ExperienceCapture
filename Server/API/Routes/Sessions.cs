/*namespace Nancy.App.Hosting.Kestrel
{
    using System;

    using MongoDB.Bson;
    using MongoDB.Bson.IO;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;

    using Nancy.App.Network;
    using Nancy.App.Random;
    using Nancy.Extensions;

    public class Sessions : NancyModule
    {
        public Sessions(IMongoDatabase db)
            : base("/sessions/")
        {
            this.Post("/", async (args) =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");

                string uniqueID = Generate.GetRandomId(4);
                var filter = Builders<BsonDocument>.Filter.Eq("id", uniqueID);
                while ((await sessions.Find(filter).FirstOrDefaultAsync()) != null)
                {
                    uniqueID = Generate.GetRandomId(4);
                    filter = Builders<BsonDocument>.Filter.Eq("id", uniqueID);
                }

                var sessionDoc = new BsonDocument
                {
                    { "id", uniqueID },
                    { "isOpen", true },
                };

                byte[] bson = sessionDoc.ToBson();
                await sessions.InsertOneAsync(sessionDoc);

                var clientDoc = new BsonDocument(sessionDoc);
                clientDoc.Remove("_id");

                string json = JsonQuery.FulfilEncoding(this.Request.Query, clientDoc);
                if (json != null)
                {
                    return json;
                }

                byte[] clientBson = clientDoc.ToBson();
                return this.Response.FromByteArray(clientBson, "application/bson");
            });

            this.Post("/{id}", async (args) =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");

                string uniqueID = (string)args.id;
                var filter = Builders<BsonDocument>.Filt    using

                    return 404;
                }

                if (sessionDoc["isOpen"] == false)
                {
                    return 400;
                }

                string collectionName = $"sessions.{uniqueID}";
                var sessionCollection = db.GetCollection<BsonDocument>(collectionName);

                BsonDocument document = JsonQuery.FulfilDencoding(this.Request.Query, this.Request.Body.AsString());
                if (document == null)
                {
                    document = BsonSerializer.Deserialize<BsonDocument>(this.Request.Body);
                }

                _ = sessionCollection.InsertOneAsync(document);

                return "OK";
            });

            this.Get("/{id}", async (args) =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");

                string uniqueID = (string)args.id;
                var filter = Builders<BsonDocument>.Filter.Eq("id", uniqueID);
                var sessionDoc = await sessions.Find(filter).FirstOrDefaultAsync();

                if (sessionDoc == null)
                {
                    return 404;
                }

                sessionDoc.Remove("_id");

                string json = JsonQuery.FulfilEncoding(this.Request.Query, sessionDoc);
                if (json != null)
                {
                    return json;
                }

                byte[] bson = sessionDoc.ToBson();
                return this.Response.FromByteArray(bson, "application/bson");
            });

            this.Delete("/{id}", async (args) =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");

                string uniqueID = (string)args.id;
                var filter = Builders<BsonDocument>.Filter.Eq("id", uniqueID);
                var update = Builders<BsonDocument>.Update.Set("isOpen", false);
                var sessionDoc = await sessions.Find(filter).FirstOrDefaultAsync();

                if (sessionDoc == null)
                {
                    return 404;
                }

                await sessions.UpdateOneAsync(filter, update);

                return "OK";
            });
        }
    }
}
*/

namespace Carter.Route.Health
{
    using Carter;
    using Carter.Request;

    using Carter.App.Generate;
    using Carter.App.Network;

    using Microsoft.AspNetCore.Http;

    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Bson.Serialization;

    public class Sessions : CarterModule
    {
        public Sessions(IMongoDatabase db)
            : base("/sessions")
        {
            this.Post("/", async (req, res) =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");

                string uniqueID = Generate.GetRandomId(4);
                var filter = Builders<BsonDocument>.Filter.Eq("id", uniqueID);
                while ((await sessions.Find(filter).FirstOrDefaultAsync()) != null)
                {
                    uniqueID = Generate.GetRandomId(4);
                    filter = Builders<BsonDocument>.Filter.Eq("id", uniqueID);
                }

                var sessionDoc = new BsonDocument
                {
                    { "id", uniqueID },
                    { "isOpen", true },
                };

                byte[] bson = sessionDoc.ToBson();
                await sessions.InsertOneAsync(sessionDoc);

                var clientDoc = new BsonDocument(sessionDoc);
                clientDoc.Remove("_id");

                string json = JsonQuery.FulfilEncoding(req.Query, clientDoc);
                if (json != null)
                {
                    JsonResponce.FromString(res, json);
                    return;
                }

                BsonResponse.FromDoc(res, clientDoc);
            });

            this.Post("/{id}", async (req, res) =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");

                string uniqueID = req.RouteValues.As<string>("id");
                var filter = Builders<BsonDocument>.Filter.Eq("id", uniqueID);
                var sessionDoc = await sessions.Find(filter).FirstOrDefaultAsync();

                if (sessionDoc == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                if (sessionDoc["isOpen"] == false)
                {
                    res.StatusCode = 400;
                    return;
                }

                string body = await req.Body.AsStringAsync();

                BsonDocument document = JsonQuery.FulfilDencoding(req.Query, body);
                if (document == null)
                {
                    document = BsonSerializer.Deserialize<BsonDocument>(body);
                }

                string collectionName = $"sessions.{uniqueID}";
                var sessionCollection = db.GetCollection<BsonDocument>(collectionName);
                _ = sessionCollection.InsertOneAsync(document);

                await res.WriteAsync("Ok");           
            });

            this.Get("/{id}", async (req, res) =>
            {
                await res.WriteAsync("The api server is running.");
            });

            this.Delete("/{id}", async (req, res) =>
            {
                await res.WriteAsync("The api server is running.");
            });
        }
    }
}