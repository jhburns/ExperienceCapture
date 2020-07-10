namespace Carter.App.Route.UsersAndAuthentication
{
    using System.Threading.Tasks;

    using Carter;

    using Carter.App.Lib.Authentication;
    using Carter.App.Lib.Environment;
    using Carter.App.Lib.Generate;
    using Carter.App.Lib.Network;
    using Carter.App.Lib.Repository;
    using Carter.App.Lib.Timer;

    using Carter.App.Route.ProtectedUsersAndAuthentication;

    using Carter.App.Validation.AccessTokenRequest;
    using Carter.App.Validation.AdminPassword;
    using Carter.App.Validation.Person;

    using Carter.ModelBinding;
    using Carter.Request;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;

    using static Microsoft.AspNetCore.Http.StatusCodes;

    public class UsersAndAuthentication : CarterModule
    {
        public UsersAndAuthentication(
            IRepository<AccessTokenSchema> accessRepo,
            IRepository<SignUpTokenSchema> signUpRepo,
            IRepository<ClaimTokenSchema> claimRepo,
            IRepository<PersonSchema> personRepo,
            IAppEnvironment env,
            IDateExtra date)
            : base("/")
        {
            this.Post("users/", async (req, res) =>
            {
                var newPerson = await req.BindAndValidate<PersonRequest>();

                if (!newPerson.ValidationResult.IsValid)
                {
                    res.StatusCode = Status400BadRequest;
                    return;
                }

                var person = await GoogleApi.ValidateUser(newPerson.Data.idToken, env);
                if (person == null)
                {
                    res.StatusCode = Status401Unauthorized;
                    return;
                }

                var filter = Builders<SignUpTokenSchema>
                        .Filter
                        .Where(t => t.Hash == PasswordHasher.Hash(newPerson.Data.signUpToken));

                var signUpDoc = await signUpRepo.FindOne(filter);

                if (signUpDoc == null || signUpDoc.CreatedAt.IsAfter(date, signUpDoc.ExpirationSeconds))
                {
                    res.StatusCode = Status401Unauthorized;
                    return;
                }

                if (!signUpDoc.IsExisting)
                {
                    res.StatusCode = Status404NotFound;
                    return;
                }

                var update = Builders<SignUpTokenSchema>.Update
                    .Set(s => s.IsExisting, false);

                var existingPerson = await personRepo.FindById(person.Subject);

                // Check if the person already exists
                if (existingPerson != null)
                {
                    // Recreate them if they where deleted
                    if (!existingPerson.IsExisting)
                    {
                        var filterPerson = Builders<PersonSchema>.Filter
                            .Where(p => p.Id == person.Subject);

                        var updatePerson = Builders<PersonSchema>.Update
                            .Set(p => p.IsExisting, true);

                        await personRepo.Update(filterPerson, updatePerson);
                        await signUpRepo.Update(filter, update);

                        await res.FromString();
                    }
                    else
                    {
                        // Return error is they really exist
                        res.StatusCode = Status409Conflict;
                    }

                    return;
                }

                var personObject = new PersonSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Id = person.Subject,
                    Fullname = person.Name,
                    Firstname = person.GivenName,
                    Lastname = person.FamilyName,
                    Email = person.Email,
                    CreatedAt = new BsonDateTime(date.UtcNow),
                    Role = signUpDoc.Role,
                };

                await personRepo.Add(personObject);
                await signUpRepo.Update(filter, update);

                await res.FromString();
            });

            this.Post("users/{id}/tokens/", async (req, res) =>
            {
                string userID = req.RouteValues.As<string>("id");
                var userDoc = await personRepo.FindById(userID);

                if (userDoc == null || !userDoc.IsExisting)
                {
                    res.StatusCode = Status404NotFound;
                    return;
                }

                var newAccessRequest = await req.BindAndValidate<AccessTokenRequest>();
                if (!newAccessRequest.ValidationResult.IsValid)
                {
                    res.StatusCode = Status400BadRequest;
                    return;
                }

                var person = await GoogleApi.ValidateUser(newAccessRequest.Data.idToken, env);
                if (person == null)
                {
                    res.StatusCode = Status401Unauthorized;
                    return;
                }

                if (person.Subject != userID)
                {
                    res.StatusCode = Status409Conflict;
                    return;
                }

                string newToken = Generate.GetRandomToken();
                string newHash = PasswordHasher.Hash(newToken);

                var tokenObject = new AccessTokenSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Hash = newHash,
                    User = userDoc.InternalId,
                    CreatedAt = new BsonDateTime(date.UtcNow),
                    Role = userDoc.Role,
                };

                await accessRepo.Add(tokenObject);

                if (newAccessRequest.Data.claimToken != null)
                {
                    var filterClaims = Builders<ClaimTokenSchema>.Filter
                        .Where(c => c.Hash == PasswordHasher.Hash(newAccessRequest.Data.claimToken));

                    var claimDoc = await claimRepo.FindOne(filterClaims);

                    // 401 returned twice, which may be hard for the client to interpret
                    if (claimDoc == null || claimDoc.CreatedAt.IsAfter(date, claimDoc.ExpirationSeconds))
                    {
                        res.StatusCode = Status401Unauthorized;
                        return;
                    }

                    // Don't allow overwriting an access token
                    if (claimDoc.Access == null && claimDoc.IsExisting)
                    {
                        var update = Builders<ClaimTokenSchema>.Update
                            .Set(c => c.Access, tokenObject.InternalId)
                            #pragma warning disable SA1515
                            // Unfortunately claim access tokens have to be saved to the database
                            // So that state can be communicated between clients
                            #pragma warning restore SA1515
                            .Set(c => c.AccessToken, newToken);

                        await claimRepo.Update(filterClaims, update);
                    }

                    await res.FromString();
                }
                else
                {
                    var responce = new AccessTokenResponce
                    {
                        AccessToken = newToken,
                        Expiration = TimerExtra.ProjectSeconds(date, tokenObject.ExpirationSeconds),
                    };

                    string json = JsonQuery.FulfilEncoding(req.Query, responce);
                    if (json != null)
                    {
                        await res.FromJson(json);
                        return;
                    }

                    await res.FromBson(responce.ToBsonDocument());
                }
            });

            this.Post("authentication/claims/", async (req, res) =>
            {
                string newToken = Generate.GetRandomToken();
                string newHash = PasswordHasher.Hash(newToken);

                var tokenDoc = new ClaimTokenSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Hash = newHash,
                    CreatedAt = new BsonDateTime(date.UtcNow),
                };

                await claimRepo.Add(tokenDoc);

                var responce = new ClaimTokenResponce
                {
                    ClaimToken = newToken,
                    Expiration = TimerExtra.ProjectSeconds(date, tokenDoc.ExpirationSeconds),
                };

                string json = JsonQuery.FulfilEncoding(req.Query, responce);
                if (json != null)
                {
                    await res.FromJson(json);
                    return;
                }

                await res.FromBson(responce.ToBsonDocument());
            });

            this.Get("authentication/claims/", async (req, res) =>
            {
                string claimToken = req.Cookies["ExperienceCapture-Claim-Token"];
                if (claimToken == null)
                {
                    res.StatusCode = Status400BadRequest;
                    return;
                }

                var filter = Builders<ClaimTokenSchema>.Filter
                    .Where(c => c.Hash == PasswordHasher.Hash(claimToken));
                var claimDoc = await claimRepo.FindOne(filter);

                if (claimDoc == null)
                {
                    res.StatusCode = Status404NotFound;
                    return;
                }

                if (!claimDoc.IsExisting || claimDoc.CreatedAt.IsAfter(date, claimDoc.ExpirationSeconds))
                {
                    res.StatusCode = Status404NotFound;
                    return;
                }

                if (claimDoc.Access == null)
                {
                    res.StatusCode = Status202Accepted;
                    await res.FromString("PENDING");
                    return;
                }

                var update = Builders<ClaimTokenSchema>.Update
                    .Set(c => c.IsExisting, false)
                    #pragma warning disable SA1515
                    // Removes the access token from the database
                    // Important to increase security
                    #pragma warning restore SA1515
                    .Set(c => c.AccessToken, null);

                await claimRepo.Update(filter, update);

                var responce = new AccessTokenResponce
                {
                    AccessToken = claimDoc.AccessToken,
                    Expiration = TimerExtra.ProjectSeconds(date, claimDoc.ExpirationSeconds),
                };

                string json = JsonQuery.FulfilEncoding(req.Query, responce);
                if (json != null)
                {
                    await res.FromString(json);
                    return;
                }

                await res.FromBson(responce.ToBsonDocument());
            });

            this.Post("authentication/admins/", async (req, res) =>
            {
                var newAdmin = await req.BindAndValidate<AdminPasswordRequest>();
                if (!newAdmin.ValidationResult.IsValid)
                {
                    res.StatusCode = Status400BadRequest;
                    return;
                }

                if (!PasswordHasher.Check(newAdmin.Data.password, env.PasswordHash) && env.SkipValidation != "true")
                {
                    res.StatusCode = Status401Unauthorized;
                    return;
                }

                string newToken = Generate.GetRandomToken();

                var tokenDoc = new SignUpTokenSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Hash = PasswordHasher.Hash(newToken),
                    CreatedAt = new BsonDateTime(date.UtcNow),
                    Role = RoleOptions.Admin,
                };

                await signUpRepo.Add(tokenDoc);

                var responce = new SignUpTokenResponce
                {
                    SignUpToken = newToken,
                    Expiration = TimerExtra.ProjectSeconds(date, tokenDoc.ExpirationSeconds),
                };

                string json = JsonQuery.FulfilEncoding(req.Query, responce);
                if (json != null)
                {
                    await res.FromJson(json);
                    return;
                }

                await res.FromBson(responce.ToBsonDocument());
            });
        }
    }

    public class PersonSchema
    {
        #pragma warning disable SA1516
        [BsonIgnoreIfNull]
        [BsonId]
        public BsonObjectId InternalId { get; set; }

        [BsonElement("id")]
        public string Id { get; set; }

        [BsonElement("fullname")]
        public string Fullname { get; set; }

        [BsonElement("firstname")]
        public string Firstname { get; set; }

        [BsonElement("lastname")]
        public string Lastname { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("isExisting")]
        public bool IsExisting { get; set; } = true;

        [BsonElement("createdAt")]
        public BsonDateTime CreatedAt { get; set; }

        [BsonElement("role")]
        public RoleOptions Role { get; set; }
        #pragma warning restore SA1516
    }

    // See Startup.cs for the code on how this is serlizalized
    #pragma warning disable SA1201
    public enum RoleOptions
    {
        Normal,
        Admin,
    }
    #pragma warning restore SA1201

    /// <inheritdoc />
    public sealed class PersonRepository : RepositoryBase<PersonSchema>
    {
        public PersonRepository(IMongoDatabase database)
            : base(database, "persons")
        {
        }

        public override async Task<PersonSchema> FindById(string id)
        {
            return await this.Collection.Find(
                Builders<PersonSchema>
                    .Filter
                    .Where(p => p.Id == id))
                    .FirstOrDefaultAsync();
        }
    }

    public class AccessTokenSchema
    {
        #pragma warning disable SA1516
        [BsonId]
        public BsonObjectId InternalId { get; set; }

        [BsonElement("hash")]
        public string Hash { get; set; }

        [BsonElement("user")]
        public BsonObjectId User { get; set; }

        [BsonElement("expirationSeconds")]
        public int ExpirationSeconds { get; set; } = 259200; // Three days

        [BsonElement("createdAt")]
        public BsonDateTime CreatedAt { get; set; }

        [BsonElement("role")]
        public RoleOptions Role { get; set; }
        #pragma warning restore SA1516
    }

    /// <inheritdoc />
    public sealed class AccessTokenRepository : RepositoryBase<AccessTokenSchema>
    {
        public AccessTokenRepository(IMongoDatabase database)
            : base(database, "persons.tokens.accesses")
        {
        }
    }

    public class AccessTokenResponce
    {
        #pragma warning disable SA1516
        [BsonElement("accessToken")]
        public string AccessToken { get; set; }

        [BsonElement("expiration")]
        public BsonDateTime Expiration { get; set; }
        #pragma warning restore SA1516
    }

    public class ClaimTokenSchema
    {
        #pragma warning disable SA1516
        [BsonId]
        public BsonObjectId InternalId { get; set; }

        [BsonElement("hash")]
        public string Hash { get; set; }

        [BsonElement("accessToken")]
        public string AccessToken { get; set; } = null;

        [BsonElement("Access")]
        public BsonObjectId Access { get; set; } = null;

        [BsonElement("expirationSeconds")]
        public int ExpirationSeconds { get; set; } = 3600; // One hour

        [BsonElement("isExisting")]
        public bool IsExisting { get; set; } = true;

        [BsonElement("createdAt")]
        public BsonDateTime CreatedAt { get; set; }
        #pragma warning restore SA1516
    }

    /// <inheritdoc />
    public sealed class ClaimTokenRepository : RepositoryBase<ClaimTokenSchema>
    {
        public ClaimTokenRepository(IMongoDatabase database)
            : base(database, "persons.tokens.claims")
        {
        }
    }

    public class ClaimTokenResponce
    {
        #pragma warning disable SA1516
        [BsonElement("claimToken")]
        public string ClaimToken { get; set; }

        [BsonElement("expiration")]
        public BsonDateTime Expiration { get; set; }
        #pragma warning restore SA1516
    }
}