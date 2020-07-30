namespace Carter.App.Route.PreSecurity
{
    using System;
    using System.Threading.Tasks;

    using Carter.App.Libs.Authentication;
    using Carter.App.Libs.Repository;
    using Carter.App.Libs.Timer;

    using Carter.App.Route.UsersAndAuthentication;

    using Microsoft.AspNetCore.Http;

    using MongoDB.Driver;

    using static Microsoft.AspNetCore.Http.StatusCodes;

    /// <summary>
    /// Sessions routes.
    /// </summary>
    public static class PreSecurity
    {
        /// <summary>
        /// Checks the access token.
        /// </summary>
        /// <returns>
        /// A function that returns true if the access token is valid.
        /// </returns>
        /// <param name="repo">Provides the access collection.</param>
        /// <param name="date">Provides the datetime.</param>
        /// <param name="minimumRole">The minimum role needed to access a module.</param>
        public static Func<HttpContext, Task<bool>> CheckAccess(
            IRepository<AccessTokenSchema> repo,
            IDateExtra date,
            RoleOptions minimumRole = RoleOptions.Normal)
        {
            return (ctx) =>
            {
                return Check(ctx.Request, ctx.Response, repo, date, minimumRole);
            };
        }

        /// <summary>
        /// A wrapper for the check static method, it keep the interface clean.
        /// </summary>
        /// <returns>
        /// True if the access token is valid.
        /// If false, the error status code is returned by pointer on the responce.
        /// </returns>
        /// <param name="req">An HTTP request.</param>
        /// <param name="res">An corresponding HTTP responce.</param>
        /// <param name="repo">Provides the access collection.</param>
        /// <param name="date">Provides the datetime.</param>
        /// <param name="minimumRole">The minimum role needed to access a module.</param>
        public static async Task<bool> CheckAccessDirectly(
            HttpRequest req,
            HttpResponse res,
            IRepository<AccessTokenSchema> repo,
            IDateExtra date,
            RoleOptions minimumRole = RoleOptions.Normal)
        {
            return await Check(req, res, repo, date, minimumRole);
        }

        /// <summary>
        /// Checks the access token, using a request and responce.
        /// </summary>
        /// <returns>
        /// True if the access token is valid.
        /// If false, the error status code is returned by pointer on the responce.
        /// </returns>
        /// <param name="req">An HTTP request.</param>
        /// <param name="res">An corresponding HTTP responce.</param>
        /// <param name="repo">Provides the access collection.</param>
        /// <param name="date">Provides the datetime.</param>
        /// <param name="minimumRole">The minimum role needed to access a module.</param>
        private static async Task<bool> Check(
            HttpRequest req,
            HttpResponse res,
            IRepository<AccessTokenSchema> repo,
            IDateExtra date,
            RoleOptions minimumRole = RoleOptions.Normal)
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
        }
    }
}