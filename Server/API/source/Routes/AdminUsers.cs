namespace Carter.App.Route.AdminUsers
{
    using System.Collections.Generic;
    using System.Linq;

    using Carter;

    using Carter.App.Lib.Environment;
    using Carter.App.Lib.Network;
    using Carter.App.Lib.Repository;
    using Carter.App.Lib.Timer;

    using Carter.App.Route.PreSecurity;
    using Carter.App.Route.ProtectedUsersAndAuthentication;
    using Carter.App.Route.UsersAndAuthentication;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;

    public class AdminUsers : CarterModule
    {
        public AdminUsers(
            IRepository<AccessTokenSchema> accessRepo,
            IRepository<SignUpTokenSchema> signUpRepo,
            IRepository<PersonSchema> personRepo,
            IAppEnvironment env,
            IDateExtra date)
            : base("/")
        {
            this.Before += PreSecurity.CheckAccess(accessRepo, date, RoleOptions.Admin);

            this.Get("users", async (req, res) =>
            {
                var filter = Builders<PersonSchema>.Filter.Empty;
                var sorter = Builders<PersonSchema>.Sort
                    .Descending(p => p.Fullname);

                var persons = await personRepo.FindAll(filter, sorter);

                var personsWithoutId = persons.Select((p) =>
                {
                    p.InternalId = null;
                    return p;
                });

                var responceBody = new PersonsResponce
                {
                    ContentList = new List<PersonSchema>(personsWithoutId),
                };

                string json = JsonQuery.FulfilEncoding(req.Query, responceBody);
                if (json != null)
                {
                    await res.FromJson(json);
                    return;
                }

                await res.FromBson(responceBody);
            });
        }
    }

    public class PersonsResponce
    {
        #pragma warning disable SA1516
        [BsonElement("contentList")]
        public List<PersonSchema> ContentList { get; set; }
        #pragma warning restore SA1516
    }
}