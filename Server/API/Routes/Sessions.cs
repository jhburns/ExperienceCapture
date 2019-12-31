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

                var accessTokens = db.GetCollection<BsonDocument>("users.tokens.access");
                string token = req.Cookies["ExperienceCapture-Access-Token"]; // Has to exist due to PreSecurity Check

                var accessTokenDoc = await accessTokens.FindEqAsync("hash", PasswordHasher.Hash(token));

                var users = db.GetCollection<BsonDocument>("users");
                var user = await users.FindEqAsync("_id", accessTokenDoc["user"].AsObjectId);

                user.Remove("_id");

                var sessionDoc = new
                {
                    isOpen = true,
                    user = user, // Copying user data instead of referencing so it can never change with the session
                    createdAt = new BsonDateTime(DateTime.Now),
                    tags = new BsonArray(),
                };

                var bsonDoc = sessionDoc.ToBsonDocument();
                bsonDoc.Add("id", uniqueID); // Added afterwards because the library will try to use it as the primary key

                await sessions.InsertOneAsync(bsonDoc);

                bsonDoc.Remove("_id");

                string json = JsonQuery.FulfilEncoding(req.Query, bsonDoc);
                if (json != null)
                {
                    JsonResponce.FromString(res, json);
                    return;
                }

                BsonResponse.FromDoc(res, bsonDoc);
            });

            this.Get("/", async (req, res) =>
            {
                var sessions = db.GetCollection<BsonDocument>("sessions");
                var projection = Builders<BsonDocument>.Projection.Exclude("_id");

                var builder = Builders<BsonDocument>.Filter;
                FilterDefinition<BsonDocument> filter = builder.Empty;

                // Three potential options: null, true, or false
                if (req.Query.As<string>("isOpen") != null)
                {
                    if (req.Query.As<bool>("isOpen"))
                    {
                        filter = filter & builder.Eq("isOpen", true);
                    }
                    else
                    {
                        filter = filter & builder.Eq("isOpen", false);
                    }
                }

                if (req.Query.As<int?>("createdWithin") != null)
                {
                    int seconds = req.Query.As<int?>("createdWithin") ?? default(int); // Should never be default to to check above
                    var range = new BsonDateTime(DateTime.Now.AddSeconds(-seconds));
                    filter = filter & builder.Gt("createdAt", range);
                }

                var sorter = Builders<BsonDocument>.Sort.Descending("createdAt");
                var sessionDocs = await sessions
                    .Find(filter)
                    .Project(projection)
                    .Sort(sorter)
                    .ToListAsync();

                var clientValues = new
                {
                    contentArray = sessionDocs, // Bson documents can't start with an array like Json, so a wrapping object is used instead
                };
                var clientDoc = clientValues.ToBsonDocument();

                string json = JsonQuery.FulfilEncoding(req.Query, clientDoc);
                if (json != null)
                {
                    JsonResponce.FromString(res, json);
                    return;
                }

                BsonResponse.FromDoc(res, clientDoc.ToBsonDocument());
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

                BsonDocument document;
                if (JsonQuery.CheckDecoding(req.Query))
                {
                    using (var ms = new MemoryStream())
                    {
                        await req.Body.CopyToAsync(ms);
                        ms.Position = 0;
                        document = BsonSerializer.Deserialize<BsonDocument>(ms);
                    }
                }
                else
                {
                    string json = await req.Body.AsStringAsync();
                    document = BsonDocument.Parse(json);
                }

                string collectionName = $"sessions.{uniqueID}";
                var sessionCollection = db.GetCollection<BsonDocument>(collectionName);

                // This one call not awaitted for max performance
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
                BasicResponce.Send(res);
            });
        }
    }
}