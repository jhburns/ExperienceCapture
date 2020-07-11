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

    /// <summary>
    /// Users routes that only administrators should be able to access.
    /// </summary>
    public class AdminUsers : CarterModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminUsers"/> class.
        /// </summary>
        /// <param name="accessRepo">Supplied through DI.</param>
        /// <param name="signUpRepo">Supplied through DI.</param>
        /// <param name="personRepo">Supplied through DI.</param>
        /// <param name="env">Supplied through DI.</param>
        /// <param name="date">Supplied through DI.</param>
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
                var filter = Builders<PersonSchema>.Filter
                    .Where(p => p.IsExisting == true);

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

    /// <summary>
    /// Responce schema for a list of users.
    /// </summary>
    public class PersonsResponce
    {
        #pragma warning disable SA1516

        /// <summary>A list of users.</summary>
        [BsonElement("contentList")]
        public List<PersonSchema> ContentList { get; set; }
        #pragma warning restore SA1516
    }
}