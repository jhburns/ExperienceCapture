namespace Carter.App.Route.Tags
{
    using Carter;

    using Carter.App.Lib.Network;
    using Carter.App.Lib.Repository;
    using Carter.App.Lib.Timer;

    using Carter.App.Route.PreSecurity;
    using Carter.App.Route.Sessions;
    using Carter.App.Route.UsersAndAuthentication;

    using Carter.Request;

    using MongoDB.Driver;

    using static Microsoft.AspNetCore.Http.StatusCodes;

    public class Tags : CarterModule
    {
        public Tags(
            IRepository<AccessTokenSchema> accessRepo,
            IRepository<SessionSchema> sessionRepo,
            IDateExtra date)
            : base("/sessions/{id}/tags")
        {
            this.Before += PreSecurity.CheckAccess(accessRepo, date);

            this.Post("/{tagName}", async (req, res) =>
            {
                string uniqueID = req.RouteValues.As<string>("id");
                var sessionDoc = await sessionRepo
                    .FindById(uniqueID);

                if (sessionDoc == null)
                {
                    res.StatusCode = Status404NotFound;
                    return;
                }

                string tag = req.RouteValues.As<string>("tagName");

                if (!sessionDoc.Tags.Contains(tag))
                {
                    sessionDoc.Tags.Add(tag);

                    var filter = Builders<SessionSchema>.Filter
                        .Where(s => s.Id == uniqueID);

                    var update = Builders<SessionSchema>.Update
                        .Set(s => s.Tags, sessionDoc.Tags);

                    await sessionRepo.Update(filter, update);
                }

                await res.FromString();
            });

            this.Delete("/{tagName}", async (req, res) =>
            {
                string uniqueID = req.RouteValues.As<string>("id");
                var sessionDoc = await sessionRepo
                    .FindById(uniqueID);

                if (sessionDoc == null)
                {
                    res.StatusCode = Status404NotFound;
                    return;
                }

                string tag = req.RouteValues.As<string>("tagName");

                if (sessionDoc.Tags.Contains(tag))
                {
                    sessionDoc.Tags.Remove(tag);

                    var filter = Builders<SessionSchema>.Filter
                        .Where(s => s.Id == uniqueID);

                    var update = Builders<SessionSchema>.Update
                        .Set(s => s.Tags, sessionDoc.Tags);

                    await sessionRepo.Update(filter, update);
                }

                await res.FromString();
            });
        }
    }
}