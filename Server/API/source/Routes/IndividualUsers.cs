namespace Carter.App.Route.ProtectedUsers
{
    using Carter;

    using Carter.App.Lib.Authentication;
    using Carter.App.Lib.Environment;
    using Carter.App.Lib.Network;
    using Carter.App.Lib.Repository;
    using Carter.App.Lib.Timer;

    using Carter.App.Route.PreSecurity;
    using Carter.App.Route.Users;

    using Carter.Request;

    using MongoDB.Driver;

    using static Microsoft.AspNetCore.Http.StatusCodes;

    public class IndividualUsers : CarterModule
    {
        public IndividualUsers(
            IRepository<AccessTokenSchema> accessRepo,
            IRepository<SignUpTokenSchema> signUpRepo,
            IRepository<PersonSchema> personRepo,
            IAppEnvironment env,
            IDateExtra date)
            : base("/users")
        {
            this.Before += PreSecurity.CheckAccess(accessRepo, date);

            this.Get("/{id}/", async (req, res) =>
            {
                string userID = req.RouteValues.As<string>("id");
                var person = await personRepo.FindById(userID);

                if (person == null)
                {
                    res.StatusCode = Status404NotFound;
                    return;
                }

                person.InternalId = null;

                // Has to exit due to pre-security check
                string token = req.Cookies["ExperienceCapture-Access-Token"];
                var accessToken = await accessRepo.FindOne(
                    Builders<AccessTokenSchema>
                        .Filter
                        .Where(a => a.Hash == PasswordHasher.Hash(token)));

                if (person.InternalId != accessToken.User)
                {
                    res.StatusCode = Status401Unauthorized;
                    return;
                }

                string json = JsonQuery.FulfilEncoding(req.Query, person);
                if (json != null)
                {
                    await res.FromJson(json);
                    return;
                }

                await res.FromBson(person);
            });

            this.Delete("/{id}/", async (req, res) =>
            {
                string userID = req.RouteValues.As<string>("id");
                var person = await personRepo.FindById(userID);

                if (person == null)
                {
                    res.StatusCode = Status404NotFound;
                    return;
                }

                person.InternalId = null;

                // Has to exit due to pre-security check
                string token = req.Cookies["ExperienceCapture-Access-Token"];
                var accessFilter = Builders<AccessTokenSchema>
                        .Filter
                        .Where(a => a.Hash == PasswordHasher.Hash(token));
                var accessToken = await accessRepo.FindOne(accessFilter);

                // Check if the user being requested is the same
                // As the one requesting, unless they have the admin role
                if (person.InternalId != accessToken.User && accessToken.Role != RoleOptions.Admin)
                {
                    res.StatusCode = Status401Unauthorized;
                    return;
                }

                var filter = Builders<PersonSchema>.Filter
                    .Where(p => p.Id == userID);

                var update = Builders<PersonSchema>.Update
                    .Set(p => p.IsExisting, false);

                await personRepo.Update(filter, update);

                await res.FromString();
            });
        }
    }
}