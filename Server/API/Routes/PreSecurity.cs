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
        public static Func<HttpRequest, HttpResponse, Task<bool>> GetSecurityCheck(IMongoDatabase db)
        {
            return async (req, res) =>
            {
                var accessTokens = db.GetCollection<BsonDocument>(AccessTokenSchema.CollectionName);

                string token = req.Cookies["ExperienceCapture-Access-Token"];
                if (token == null)
                {
                    res.StatusCode = 400;
                    return false;
                }

                var accessDoc = await accessTokens.FindEqAsync("hash", PasswordHasher.Hash(token));

                if (accessDoc == null || accessDoc["createdAt"].IsAfter(accessDoc["expirationSeconds"]))
                {
                    res.StatusCode = 401;
                    return false;
                }

                return true;
            };
        }
    }
}