namespace Carter.App.Route.Tags
{
    using Carter;

    using Carter.App.Libs.Network;
    using Carter.App.Libs.Repository;
    using Carter.App.Libs.Timer;

    using Carter.App.MetaData.Tags;

    using Carter.App.Route.PreSecurity;
    using Carter.App.Route.Sessions;
    using Carter.App.Route.UsersAndAuthentication;

    using Carter.Request;

    using MongoDB.Driver;

    using static Microsoft.AspNetCore.Http.StatusCodes;

    /// <summary>
    /// Tag routes.
    /// </summary>
    public class Tags : CarterModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tags"/> class.
        /// </summary>
        /// <param name="accessRepo">Supplied through DI.</param>
        /// <param name="sessionRepo">Supplied through DI.</param>
        /// <param name="date">Supplied through DI.</param>
        public Tags(
            IRepository<AccessTokenSchema> accessRepo,
            IRepository<SessionSchema> sessionRepo,
            IDateExtra date)
            : base("/sessions/{id}/tags")
        {
            this.Before += PreSecurity.CheckAccess(accessRepo, date);

            this.Post<PostTags>("/{tagName}", async (req, res) =>
            {
                string shortID = req.RouteValues.As<string>("id");
                var sessionDoc = await sessionRepo
                    .FindById(shortID);

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
                        .Where(s => s.Id == shortID);

                    var update = Builders<SessionSchema>.Update
                        .Set(s => s.Tags, sessionDoc.Tags);

                    await sessionRepo.Update(filter, update);
                }

                await res.FromString();
            });

            this.Delete<DeleteTags>("/{tagName}", async (req, res) =>
            {
                string shortID = req.RouteValues.As<string>("id");
                var sessionDoc = await sessionRepo
                    .FindById(shortID);

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
                        .Where(s => s.Id == shortID);

                    var update = Builders<SessionSchema>.Update
                        .Set(s => s.Tags, sessionDoc.Tags);

                    await sessionRepo.Update(filter, update);
                }

                await res.FromString();
            });
        }
    }
}