namespace Carter.App.Route.PreSecurity
{
    using System;
    using System.Threading.Tasks;

    using Carter.App.Lib.Authentication;
    using Carter.App.Lib.Mongo;
    using Carter.App.Lib.Timer;

    using Carter.App.Route.Users;

    using Microsoft.AspNetCore.Http;

    using MongoDB.Bson;
    using MongoDB.Driver;

    public static class PreSecurity
    {
        public static Func<HttpContext, Task<bool>> GetSecurityCheck(IMongoDatabase db)
        {   
            Func<HttpRequest, HttpResponse, Task<bool>> check = async (req, res) =>
            {
                var accessTokens = db.GetCollection<AccessTokenSchema>(AccessTokenSchema.CollectionName);

                string token = req.Cookies["ExperienceCapture-Access-Token"];
                if (token == null)
                {
                    res.StatusCode = 400;
                    return false;
                }

                var accessTokenDoc = await (await accessTokens.FindAsync(
                    Builders<AccessTokenSchema>
                        .Filter
                        .Where(a => a.Hash == PasswordHasher.Hash(token))))
                        .FirstOrDefaultAsync();

                if (accessTokenDoc == null 
                    || accessTokenDoc.CreatedAt.IsAfter(accessTokenDoc.ExpirationSeconds))
                {
                    res.StatusCode = 401;
                    return false;
                }

                return true;
            };

            return async (ctx) => {
                return await check(ctx.Request, ctx.Response);
            };
        }
    }
}