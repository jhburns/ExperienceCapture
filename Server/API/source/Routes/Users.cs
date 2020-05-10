namespace Carter.App.Route.Users
{
    using System;

    using Carter;

    using Carter.App.Lib.Authentication;
    using Carter.App.Lib.Environment;
    using Carter.App.Lib.Generate;
    using Carter.App.Lib.Network;
    using Carter.App.Lib.Timer;

    using Carter.App.Route.NewSignUp;

    using Carter.App.Validation.AccessTokenRequest;
    using Carter.App.Validation.AdminPassword;
    using Carter.App.Validation.Person;

    using Carter.ModelBinding;
    using Carter.Request;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;

    public class Users : CarterModule
    {
        public Users(IMongoDatabase db, IAppEnvironment env)
            : base("/users")
        {
            this.Post("/", async (req, res) =>
            {
                var newPerson = await req.BindAndValidate<Person>();

                if (!newPerson.ValidationResult.IsValid)
                {
                    res.StatusCode = 400;
                    return;
                }

                var person = await GoogleApi.ValidateUser(newPerson.Data.idToken, env);
                if (person == null)
                {
                    res.StatusCode = 401;
                    return;
                }

                var signUpTokens = db.GetCollection<SignUpTokenSchema>(SignUpTokenSchema.CollectionName);

                var signUpDoc = await signUpTokens.Find(
                    Builders<SignUpTokenSchema>
                        .Filter
                        .Where(t => t.Hash == PasswordHasher.Hash(newPerson.Data.signUpToken)))
                        .FirstOrDefaultAsync();

                if (signUpDoc == null || signUpDoc.CreatedAt.IsAfter(signUpDoc.ExpirationSeconds))
                {
                    res.StatusCode = 401;
                    return;
                }

                var users = db.GetCollection<PersonSchema>(PersonSchema.CollectionName);

                var existingPerson = await users.Find(
                    Builders<PersonSchema>
                        .Filter
                        .Where(p => p.Id == person.Subject))
                        .FirstOrDefaultAsync();

                if (existingPerson != null)
                {
                    res.StatusCode = 409;
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
                    CreatedAt = new BsonDateTime(DateTime.Now),
                };

                var personDoc = personObject.ToBsonDocument();

                await users.InsertOneAsync(personObject);

                await res.FromString();
            });

            this.Post("/{id}/tokens/", async (req, res) =>
            {
                var users = db.GetCollection<PersonSchema>(PersonSchema.CollectionName);

                string userID = req.RouteValues.As<string>("id");
                var userDoc = await users.Find(
                    Builders<PersonSchema>
                        .Filter
                        .Where(p => p.Id == userID))
                        .FirstOrDefaultAsync();

                if (userDoc == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                var newAccessRequest = await req.BindAndValidate<AccessToken>();
                if (!newAccessRequest.ValidationResult.IsValid)
                {
                    res.StatusCode = 400;
                    return;
                }

                var person = await GoogleApi.ValidateUser(newAccessRequest.Data.idToken, env);
                if (person == null)
                {
                    res.StatusCode = 401;
                    return;
                }

                if (person.Subject != userID)
                {
                    res.StatusCode = 409;
                    return;
                }

                string newToken = Generate.GetRandomToken();
                string newHash = PasswordHasher.Hash(newToken);
                var accessTokens = db.GetCollection<AccessTokenSchema>(AccessTokenSchema.CollectionName);

                var tokenObject = new AccessTokenSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Hash = newHash,
                    User = userDoc.InternalId,
                    CreatedAt = new BsonDateTime(DateTime.Now),
                };

                await accessTokens.InsertOneAsync(tokenObject);

                if (newAccessRequest.Data.claimToken != null)
                {
                    var claimTokens = db.GetCollection<ClaimTokenSchema>(ClaimTokenSchema.CollectionName);

                    var filterClaims = Builders<ClaimTokenSchema>.Filter
                        .Where(c => c.Hash == PasswordHasher.Hash(newAccessRequest.Data.claimToken));

                    var claimDoc = await claimTokens.Find(filterClaims).FirstOrDefaultAsync();

                    // 401 returned twice, which may be hard for the client to interpret
                    if (claimDoc == null || claimDoc.CreatedAt.IsAfter(claimDoc.ExpirationSeconds))
                    {
                        res.StatusCode = 401;
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

                        await claimTokens.UpdateOneAsync(filterClaims, update);
                    }

                    await res.FromString();
                }
                else
                {
                    var responce = new
                    {
                        accessToken = newToken,
                    };
                    var responceDoc = responce.ToBsonDocument();

                    string json = JsonQuery.FulfilEncoding(req.Query, responceDoc);
                    if (json != null)
                    {
                        await res.FromJson(json);
                        return;
                    }

                    await res.FromBson(responceDoc);
                }
            });

            this.Post("/claims/", async (req, res) =>
            {
                string newToken = Generate.GetRandomToken();
                string newHash = PasswordHasher.Hash(newToken);
                var claimTokens = db.GetCollection<ClaimTokenSchema>(ClaimTokenSchema.CollectionName);

                var tokenDoc = new ClaimTokenSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Hash = newHash,
                    CreatedAt = new BsonDateTime(DateTime.Now),
                };

                await claimTokens.InsertOneAsync(tokenDoc);

                var responce = new
                {
                    claimToken = newToken,
                };
                var responceDoc = responce.ToBsonDocument();

                string json = JsonQuery.FulfilEncoding(req.Query, responceDoc);
                if (json != null)
                {
                    await res.FromJson(json);
                    return;
                }

                await res.FromBson(responceDoc);
            });

            this.Get("/claims/", async (req, res) =>
            {
                string claimToken = req.Cookies["ExperienceCapture-Claim-Token"];
                if (claimToken == null)
                {
                    res.StatusCode = 400;
                    return;
                }

                var claimTokens = db.GetCollection<ClaimTokenSchema>(ClaimTokenSchema.CollectionName);
                var filter = Builders<ClaimTokenSchema>.Filter
                    .Where(c => c.Hash == PasswordHasher.Hash(claimToken));
                var claimDoc = await claimTokens.Find(filter).FirstOrDefaultAsync();

                if (claimDoc == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                if (!claimDoc.IsExisting || claimDoc.CreatedAt.IsAfter(claimDoc.ExpirationSeconds))
                {
                    res.StatusCode = 404;
                    return;
                }

                if (claimDoc.Access == null)
                {
                    res.StatusCode = 202;
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

                await claimTokens.UpdateOneAsync(filter, update);

                var responce = new
                {
                    accessToken = claimDoc.AccessToken,
                };
                var responceDoc = responce.ToBsonDocument();

                string json = JsonQuery.FulfilEncoding(req.Query, responceDoc);
                if (json != null)
                {
                    await res.FromString(json);
                    return;
                }

                await res.FromBson(responceDoc);
            });

            this.Post("/signUp/admin/", async (req, res) =>
            {
                var newAdmin = await req.BindAndValidate<AdminPassword>();
                if (!newAdmin.ValidationResult.IsValid)
                {
                    res.StatusCode = 400;
                    return;
                }

                if (!PasswordHasher.Check(newAdmin.Data.password, env.PasswordHash))
                {
                    res.StatusCode = 401;
                    return;
                }

                string newToken = Generate.GetRandomToken();
                var signUpTokens = db.GetCollection<SignUpTokenSchema>(SignUpTokenSchema.CollectionName);

                var tokenDoc = new SignUpTokenSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Hash = PasswordHasher.Hash(newToken),
                    CreatedAt = new BsonDateTime(DateTime.Now),
                };

                await signUpTokens.InsertOneAsync(tokenDoc);

                var responce = new
                {
                    claimToken = newToken,
                };
                var responceDoc = responce.ToBsonDocument();

                string json = JsonQuery.FulfilEncoding(req.Query, responceDoc);
                if (json != null)
                {
                    await res.FromJson(json);
                    return;
                }

                await res.FromBson(responceDoc);
            });
        }
    }

    public class PersonSchema
    {
        #pragma warning disable SA1516
        [BsonIgnore]
        public const string CollectionName = "persons";

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

        [BsonElement("createdAt")]
        public BsonDateTime CreatedAt { get; set; }
        #pragma warning restore SA151, SA1300
    }

    public class AccessTokenSchema
    {
        #pragma warning disable SA1516
        [BsonIgnore]
        public const string CollectionName = "persons.tokens.accesses";

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
        #pragma warning restore SA151, SA1300
    }

    public class ClaimTokenSchema
    {
        #pragma warning disable SA1516
        [BsonIgnore]
        public const string CollectionName = "persons.tokens.claims";

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
        #pragma warning restore SA151, SA1300
    }
}