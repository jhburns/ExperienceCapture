namespace Carter.App.Route.Sessions
{
    using Carter;

    using Carter.App.Lib.Generate;
    using Carter.App.Lib.Network;
    using Carter.App.Lib.Mongo;
    using Carter.App.Route.PreSecurity;

    using Carter.Request;

    using Microsoft.AspNetCore.Http;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;

    public class Sessions : CarterModule
    {
        public Sessions(IMongoDatabase db)
            : base("/sessions")
        {
            this.Before += PreSecurity.GetSecurityCheck(db);

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
                var sessionDoc = await sessions.FindEqAsync("id", uniqueID);

                if (sessionDoc == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                if (!sessionDoc["isOpen"].AsBoolean)
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

                res.ContentType = "application/text; charset=utf-8";
                await res.WriteAsync("OK");
            });

            this.Get("/{id}", async (req, res) =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");

                string uniqueID = req.RouteValues.As<string>("id");
                var sessionDoc = await sessions.FindEqAsync("id", uniqueID);

                if (sessionDoc == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                sessionDoc.Remove("_id");

                string json = JsonQuery.FulfilEncoding(req.Query, sessionDoc);
                if (json != null)
                {
                    JsonResponce.FromString(res, json);
                    return;
                }

                BsonResponse.FromDoc(res, sessionDoc);
            });

            this.Delete("/{id}", async (req, res) =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");

                string uniqueID = req.RouteValues.As<string>("id");
                var filter = Builders<BsonDocument>.Filter.Eq("id", uniqueID);
                var update = Builders<BsonDocument>.Update.Set("isOpen", false);
                var sessionDoc = await sessions.Find(filter).FirstOrDefaultAsync();

                if (sessionDoc == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                await sessions.UpdateOneAsync(filter, update);

                res.ContentType = "application/text; charset=utf-8";
                await res.WriteAsync("OK");
            });
        }
    }
}