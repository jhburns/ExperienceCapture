namespace Carter.App.Route.Tags
{
    using Carter;

    using Carter.App.Lib.Network;
    using Carter.App.Lib.Repository;

    using Carter.App.Route.PreSecurity;
    using Carter.App.Route.Sessions;
    using Carter.App.Route.Users;

    using Carter.Request;

    using MongoDB.Bson;
    using MongoDB.Driver;

    public class Tags : CarterModule
    {
        public Tags(
            IRepository<AccessTokenSchema> accessRepo,
            IRepository<SessionSchema> sessionRepo)
            : base("/sessions/{id}/tags")
        {
            this.Before += PreSecurity.GetSecurityCheck(accessRepo);

            this.Post("/{tagName}", async (req, res) =>
            {
                string uniqueID = req.RouteValues.As<string>("id");
                var filter = Builders<SessionSchema>.Filter.Where(s => s.Id == uniqueID);
                var sessionDoc = await sessionRepo
                    .FindOne(filter);

                if (sessionDoc == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                string tag = req.RouteValues.As<string>("tagName");

                if (!sessionDoc.Tags.Contains(tag))
                {
                    sessionDoc.Tags.Add(tag);
                    var update = Builders<SessionSchema>.Update
                        .Set(s => s.Tags, sessionDoc.Tags);
                    await sessionRepo.Update(filter, update);
                }

                await res.FromString();
            });

            this.Delete("/{tagName}", async (req, res) =>
            {
                string uniqueID = req.RouteValues.As<string>("id");
                var filter = Builders<SessionSchema>.Filter.Where(s => s.Id == uniqueID);
                var sessionDoc = await sessionRepo
                    .FindOne(filter);

                if (sessionDoc == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                string tag = req.RouteValues.As<string>("tagName");

                if (sessionDoc.Tags.Contains(tag))
                {
                    sessionDoc.Tags.Remove(tag);
                    var update = Builders<SessionSchema>.Update
                        .Set(s => s.Tags, sessionDoc.Tags);
                    await sessionRepo.Update(filter, update);
                }

                await res.FromString();
            });
        }
    }
}