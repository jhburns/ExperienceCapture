namespace Carter.App.Route.PreSecurity
{
    using System;
    using System.Threading.Tasks;

    using Carter.App.Lib.Authentication;
    using Carter.App.Lib.Timer;

    using Microsoft.AspNetCore.Http;

    using MongoDB.Bson;
    using MongoDB.Driver;

    public static class PreSecurity
    {
        public static Func<HttpContext, Task<bool>> GetSecurityCheck(IMongoDatabase db)
        {
            return async (ctx) =>
            {
                var accessTokens = db.GetCollection<BsonDocument>("users.tokens.access");

                string token = ctx.Request.Cookies["ExperienceCapture-Access-Token"];
                if (token == null)
                {
                    ctx.Response.StatusCode = 400;
                    return false;
                }

                var filter = Builders<BsonDocument>.Filter.Eq("hash", PasswordHasher.Hash(token));
                var accessDoc = await accessTokens.Find(filter).FirstOrDefaultAsync();

                if (accessDoc == null || accessDoc["createdAt"].IsAfter(accessDoc["expirationSeconds"]))
                {
                    ctx.Response.StatusCode = 401;
                    return false;
                }

                return true;
            };
        }
    }
}