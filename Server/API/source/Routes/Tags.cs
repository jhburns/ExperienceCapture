namespace Carter.App.Route.Tags
{
    using Carter;

    using Carter.App.Lib.Network;
    using Carter.App.Route.PreSecurity;

    using Carter.App.Route.Sessions;

    using Carter.Request;

    using MongoDB.Bson;
    using MongoDB.Driver;

    public class Tags : CarterModule
    {
        public Tags(IMongoDatabase db)
            : base("/sessions/{id}/tags")
        {
            this.Before += PreSecurity.GetSecurityCheck(db);

            this.Post("/{tagName}", async (req, res) =>
            {
                var sessions = db.GetCollection<SessionSchema>(SessionSchema.CollectionName);

                string uniqueID = req.RouteValues.As<string>("id");
                var filter = Builders<SessionSchema>.Filter.Where(s => s.Id == uniqueID);
                var sessionDoc = await sessions.Find(filter).FirstOrDefaultAsync();

                if (sessionDoc == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                string collectionName = $"sessions.{uniqueID}";
                var sessionCollection = db.GetCollection<BsonDocument>(collectionName);

                string tag = req.RouteValues.As<string>("tagName");

                if (!sessionDoc.Tags.Contains(tag))
                {
                    sessionDoc.Tags.Add(tag);
                    var update = Builders<SessionSchema>.Update
                        .Set(s => s.Tags, sessionDoc.Tags);
                    await sessions.UpdateOneAsync(filter, update);
                }

                await res.FromString();
            });

            this.Delete("/{tagName}", async (req, res) =>
            {
                var sessions = db.GetCollection<SessionSchema>(SessionSchema.CollectionName);

                string uniqueID = req.RouteValues.As<string>("id");
                var filter = Builders<SessionSchema>.Filter.Where(s => s.Id == uniqueID);
                var sessionDoc = await sessions.Find(filter).FirstOrDefaultAsync();

                if (sessionDoc == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                string collectionName = $"sessions.{uniqueID}";
                var sessionCollection = db.GetCollection<BsonDocument>(collectionName);

                string tag = req.RouteValues.As<string>("tagName");

                if (sessionDoc.Tags.Contains(tag))
                {
                    sessionDoc.Tags.Remove(tag);
                    var update = Builders<SessionSchema>.Update
                        .Set(s => s.Tags, sessionDoc.Tags);
                    await sessions.UpdateOneAsync(filter, update);
                }

                await res.FromString();
            });
        }
    }
}