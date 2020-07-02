namespace Carter.App.Route.PreSecurity
{
    using System;
    using System.Threading.Tasks;

    using Carter.App.Lib.Authentication;
    using Carter.App.Lib.Repository;
    using Carter.App.Lib.Timer;

    using Carter.App.Route.Users;

    using Microsoft.AspNetCore.Http;

    using MongoDB.Driver;

    using static Microsoft.AspNetCore.Http.StatusCodes;

    public static class PreSecurity
    {
        public static Func<HttpContext, Task<bool>> GetSecurityCheck(
            IRepository<AccessTokenSchema> repo,
            IDateExtra date,
            RoleOptions minimumRole = RoleOptions.Normal)
        {
            Func<HttpRequest, HttpResponse, Task<bool>> check = async (req, res) =>
            {
                string token = req.Cookies["ExperienceCapture-Access-Token"];
                if (token == null)
                {
                    res.StatusCode = Status400BadRequest;
                    return false;
                }

                var accessTokenDoc = await repo.FindOne(
                    Builders<AccessTokenSchema>
                        .Filter
                        .Where(a => a.Hash == PasswordHasher.Hash(token)));

                if (accessTokenDoc == null
                    || accessTokenDoc.CreatedAt.IsAfter(date, accessTokenDoc.ExpirationSeconds)
                    || (int)accessTokenDoc.Role < (int)minimumRole)
                {
                    res.StatusCode = Status401Unauthorized;
                    return false;
                }

                return true;
            };

            return async (ctx) =>
            {
                return await check(ctx.Request, ctx.Response);
            };
        }
    }
}