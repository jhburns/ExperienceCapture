namespace Carter.App.Route.Users
{
    using System;

    using Carter;

    using Carter.App.Lib.Authentication;
    using Carter.App.Lib.CustomExceptions;
    using Carter.App.Lib.Generate;
    using Carter.App.Lib.Mongo;
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
        private static readonly string PasswordHash = Environment.GetEnvironmentVariable("admin_password_hash")
            ?? throw new EnviromentVarNotSet("The following is unset", "admin_password_hash");

        public Users(IMongoDatabase db)
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

                var person = await GoogleApi.ValidateUser(newPerson.Data.idToken);
                if (person == null)
                {
                    res.StatusCode = 401;
                    return;
                }

                var signUpTokens = db.GetCollection<BsonDocument>(SignUpTokenSchema.CollectionName);

                var signUpDoc = await signUpTokens.FindEqAsync("hash", PasswordHasher.Hash(newPerson.Data.signUpToken));

                // 401 returned twice which may make interpretting the code harder for a client
                if (signUpDoc == null || signUpDoc["createdAt"].IsAfter(signUpDoc["expirationSeconds"]))
                {
                    res.StatusCode = 401;
                    return;
                }

                var users = db.GetCollection<PersonSchema>("users");

                var existingPerson = await (await users.FindAsync(
                    Builders<PersonSchema>
                        .Filter
                        .Where(p => p.Id == person.Subject)))
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
                BasicResponce.Send(res);
            });

            // TODO: refactor this section
            this.Post("/{id}/tokens/", async (req, res) =>
            {
                var users = db.GetCollection<BsonDocument>(PersonSchema.CollectionName);

                string userID = req.RouteValues.As<string>("id");
                var userDoc = await users.FindEqAsync("id", userID);

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

                var person = await GoogleApi.ValidateUser(newAccessRequest.Data.idToken);
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
                var accessTokens = db.GetCollection<BsonDocument>(AccessTokenSchema.CollectionName);

                var tokenObject = new
                {
                    hash = newHash,
                    user = userDoc["_id"],
                    expirationSeconds = 259200, // Three days
                    createdAt = new BsonDateTime(DateTime.Now),
                };

                BsonDocument tokenDoc = tokenObject.ToBsonDocument();

                await accessTokens.InsertOneAsync(tokenDoc);

                if (newAccessRequest.Data.claimToken != null)
                {
                    var claimTokens = db.GetCollection<BsonDocument>(ClaimTokenSchema.CollectionName);

                    var filterClaims = Builders<BsonDocument>.Filter
                        .Eq("hash", PasswordHasher.Hash(newAccessRequest.Data.claimToken));

                    var claimDoc = await claimTokens.Find(filterClaims).FirstOrDefaultAsync();

                    // 401 returned twice, which may be hard for the client to interpret
                    if (claimDoc == null || claimDoc["createdAt"].IsAfter(claimDoc["expirationSeconds"]))
                    {
                        res.StatusCode = 401;
                        return;
                    }

                    // Don't allow overwriting an access token
                    if (claimDoc["isPending"].AsBoolean && claimDoc["isExisting"].AsBoolean)
                    {
                        var update = Builders<BsonDocument>.Update
                            .Set("isPending", false)
                            .Set("access", tokenDoc["_id"])
                            #pragma warning disable SA1515
                            // Unfortunately claim access tokens have to be saved to the database
                            // So that state can be communicated between clients
                            #pragma warning restore SA1515
                            .Set("accessToken", newToken);

                        await claimTokens.UpdateOneAsync(filterClaims, update);
                    }

                    BasicResponce.Send(res);
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
                        JsonResponce.FromString(res, json);
                        return;
                    }

                    BsonResponse.FromDoc(res, responceDoc);
                }
            });

            this.Post("/claims/", async (req, res) =>
            {
                string newToken = Generate.GetRandomToken();
                string newHash = PasswordHasher.Hash(newToken);
                var accessTokens = db.GetCollection<BsonDocument>(ClaimTokenSchema.CollectionName);

                var tokenDoc = new
                {
                    hash = newHash,
                    expirationSeconds = 3600, // One hour
                    isPending = true,
                    isExisting = true,
                    createdAt = new BsonDateTime(DateTime.Now),
                };

                await accessTokens.InsertOneAsync(tokenDoc.ToBsonDocument());

                var responce = new
                {
                    claimToken = newToken,
                };
                var responceDoc = responce.ToBsonDocument();

                string json = JsonQuery.FulfilEncoding(req.Query, responceDoc);
                if (json != null)
                {
                    JsonResponce.FromString(res, json);
                    return;
                }

                BsonResponse.FromDoc(res, responceDoc);
            });

            this.Get("/claims/", async (req, res) =>
            {
                string claimToken = req.Cookies["ExperienceCapture-Claim-Token"];
                if (claimToken == null)
                {
                    res.StatusCode = 400;
                    return;
                }

                var claimTokens = db.GetCollection<BsonDocument>(ClaimTokenSchema.CollectionName);
                var filter = Builders<BsonDocument>.Filter.Eq("hash", PasswordHasher.Hash(claimToken));
                var claimDoc = await claimTokens.Find(filter).FirstOrDefaultAsync();

                if (claimDoc == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                // 404 returned twice, may be hard for client to understand
                if (!claimDoc["isExisting"].AsBoolean || claimDoc["createdAt"].IsAfter(claimDoc["expirationSeconds"]))
                {
                    res.StatusCode = 404;
                    return;
                }

                if (claimDoc["isPending"].AsBoolean)
                {
                    res.StatusCode = 202;
                    BasicResponce.Send(res, "PENDING");
                    return;
                }

                var update = Builders<BsonDocument>.Update
                    .Set("isExisting", false)
                    #pragma warning disable SA1515
                    // Removes the access token from the database
                    // Important to increase security
                    #pragma warning restore SA1515
                    .Unset("accessToken");

                await claimTokens.UpdateOneAsync(filter, update);

                var responce = new
                {
                    accessToken = claimDoc["accessToken"].AsString,
                };
                var responceDoc = responce.ToBsonDocument();

                string json = JsonQuery.FulfilEncoding(req.Query, responceDoc);
                if (json != null)
                {
                    JsonResponce.FromString(res, json);
                    return;
                }

                BsonResponse.FromDoc(res, responceDoc);
            });

            this.Post("/signUp/admin/", async (req, res) =>
            {
                var newAdmin = await req.BindAndValidate<AdminPassword>();
                if (!newAdmin.ValidationResult.IsValid)
                {
                    res.StatusCode = 400;
                    return;
                }

                if (!PasswordHasher.Check(newAdmin.Data.password, PasswordHash))
                {
                    res.StatusCode = 401;
                    return;
                }

                string newToken = Generate.GetRandomToken();
                var signUpTokens = db.GetCollection<BsonDocument>(ClaimTokenSchema.CollectionName);

                var tokenDoc = new
                {
                    hash = PasswordHasher.Hash(newToken),
                    expirationSeconds = 3600, // One hour
                    createdAt = new BsonDateTime(DateTime.Now),
                };

                await signUpTokens.InsertOneAsync(tokenDoc.ToBsonDocument());

                var responce = new
                {
                    claimToken = newToken,
                };
                var responceDoc = responce.ToBsonDocument();

                string json = JsonQuery.FulfilEncoding(req.Query, responceDoc);
                if (json != null)
                {
                    JsonResponce.FromString(res, json);
                    return;
                }

                BsonResponse.FromDoc(res, responceDoc);
            });
        }
    }

    public class PersonSchema
    {
        #pragma warning disable SA1516
        [BsonIgnore]
        public const string CollectionName = "persons";

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
        public string AccessToken { get; set; }

        [BsonElement("expirationSeconds")]
        public int ExpirationSeconds { get; set; } = 3600; // One hour

        [BsonElement("isPending")]
        public bool IsPending { get; set; } = false;

        [BsonElement("isExisting")]
        public bool IsExisting { get; set; } = true;

        [BsonElement("createdAt")]
        public BsonDateTime CreatedAt { get; set; }
        #pragma warning restore SA151, SA1300
    }
}