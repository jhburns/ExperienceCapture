namespace Carter.Tests.Route.ProtectedUsersAndAuthentication
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Carter.App.Lib.Repository;
    using Carter.App.Route.ProtectedUsersAndAuthentication;
    using Carter.App.Route.UsersAndAuthentication;

    using Carter.Tests.HostingExtra;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;

    using Moq;

    using Xunit;

    public class ProtectedUsersAndAuthenticationTests
    {
        [Fact]
        public async Task RequiresAccessGetUser()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Get, "/users/1234567890987654321/", false);
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Getting a user is not a bad request without access token.");
        }

        [Fact]
        public async Task MissingUserIsNotFoundGetUser()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Get, "/users/1234567890987654321/", false);
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Getting a user is not 'not found' when missing.");
        }

        [Fact]
        public async Task NonSameUserIsUnauthorizedGetUser()
        {
            var firstId = ObjectId.GenerateNewId();
            var secondId = ObjectId.GenerateNewId();

            var accessMock = new Mock<IRepository<AccessTokenSchema>>();
            var result = new Task<AccessTokenSchema>(() =>
            {
                return new AccessTokenSchema
                {
                    CreatedAt = new BsonDateTime(DateTime.UtcNow.AddSeconds(86400)),
                    Role = RoleOptions.Normal,
                    User = firstId,
                };
            });
            result.Start();

            accessMock.Setup(a => a.FindOne(It.IsAny<FilterDefinition<AccessTokenSchema>>()))
                .Returns(result)
                .Verifiable();

            var personMock = new Mock<IRepository<PersonSchema>>();
            var personResult = new Task<PersonSchema>(() =>
            {
                return new PersonSchema()
                {
                    InternalId = secondId,
                };
            });
            personResult.Start();

            personMock.Setup(p => p.FindById(It.IsAny<string>()))
                .Returns(personResult)
                .Verifiable("A person is never searched for.");

            var client = CustomHost.Create(accessMock: accessMock, personMock: personMock);
            var request = CustomRequest.Create(HttpMethod.Get, "/users/1234567890987654321/");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.Unauthorized,
                "Getting a user is not unauthorized when it is not the same.");
        }

        [Fact]
        public async Task AdminIsAllowsAuthorizedGetUser()
        {
            var firstId = ObjectId.GenerateNewId();
            var secondId = ObjectId.GenerateNewId();

            var accessMock = new Mock<IRepository<AccessTokenSchema>>();
            var result = new Task<AccessTokenSchema>(() =>
            {
                return new AccessTokenSchema
                {
                    CreatedAt = new BsonDateTime(DateTime.UtcNow.AddSeconds(86400)),
                    Role = RoleOptions.Admin,
                    User = firstId,
                };
            });
            result.Start();

            accessMock.Setup(a => a.FindOne(It.IsAny<FilterDefinition<AccessTokenSchema>>()))
                .Returns(result)
                .Verifiable();

            var personMock = new Mock<IRepository<PersonSchema>>();
            var personResult = new Task<PersonSchema>(() =>
            {
                return new PersonSchema()
                {
                    InternalId = secondId,
                };
            });
            personResult.Start();

            personMock.Setup(p => p.FindById(It.IsAny<string>()))
                .Returns(personResult)
                .Verifiable("A person is never searched for.");

            var client = CustomHost.Create(accessMock: accessMock, personMock: personMock);
            var request = CustomRequest.Create(HttpMethod.Get, "/users/1234567890987654321/");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ResponceIsValidGetUser()
        {
            var accessMock = new Mock<IRepository<AccessTokenSchema>>();
            var result = new Task<AccessTokenSchema>(() =>
            {
                return new AccessTokenSchema
                {
                    CreatedAt = new BsonDateTime(DateTime.UtcNow.AddSeconds(86400)),
                    Role = RoleOptions.Admin,
                };
            });
            result.Start();

            accessMock.Setup(a => a.FindOne(It.IsAny<FilterDefinition<AccessTokenSchema>>()))
                .Returns(result)
                .Verifiable();

            var personMock = new Mock<IRepository<PersonSchema>>();
            var personResult = new Task<PersonSchema>(() =>
            {
                return new PersonSchema()
                {
                };
            });
            personResult.Start();

            personMock.Setup(p => p.FindById(It.IsAny<string>()))
                .Returns(personResult)
                .Verifiable("A person is never searched for.");

            var client = CustomHost.Create(accessMock: accessMock, personMock: personMock);
            var request = CustomRequest.Create(HttpMethod.Get, "/users/1234567890987654321/");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var data = BsonSerializer.Deserialize<PersonSchema>(body);
        }

        [Fact]
        public async Task ResponceIsValidBsonGetUser()
        {
            var accessMock = new Mock<IRepository<AccessTokenSchema>>();
            var result = new Task<AccessTokenSchema>(() =>
            {
                return new AccessTokenSchema
                {
                    CreatedAt = new BsonDateTime(DateTime.UtcNow.AddSeconds(86400)),
                    Role = RoleOptions.Admin,
                };
            });
            result.Start();

            accessMock.Setup(a => a.FindOne(It.IsAny<FilterDefinition<AccessTokenSchema>>()))
                .Returns(result)
                .Verifiable();

            var personMock = new Mock<IRepository<PersonSchema>>();
            var personResult = new Task<PersonSchema>(() =>
            {
                return new PersonSchema()
                {
                };
            });
            personResult.Start();

            personMock.Setup(p => p.FindById(It.IsAny<string>()))
                .Returns(personResult)
                .Verifiable("A person is never searched for.");

            var client = CustomHost.Create(accessMock: accessMock, personMock: personMock);
            var request = CustomRequest.Create(HttpMethod.Get, "/users/1234567890987654321?bson=true");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsByteArrayAsync();
            var data = BsonSerializer.Deserialize<PersonSchema>(body);
        }

        [Fact]
        public async Task RequiresAccessDeleteUser()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Delete, "/users/1234567890987654321/", false);
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Deleting a user is not a bad request without access token.");
        }

        [Fact]
        public async Task MissingUserIsNotFoundDeleteUser()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Delete, "/users/1234567890987654321/", false);
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Getting a user is not 'not found' when missing.");
        }

        [Fact]
        public async Task NonSameUserIsUnauthorizedDeleteUser()
        {
            var firstId = ObjectId.GenerateNewId();
            var secondId = ObjectId.GenerateNewId();

            var accessMock = new Mock<IRepository<AccessTokenSchema>>();
            var result = new Task<AccessTokenSchema>(() =>
            {
                return new AccessTokenSchema
                {
                    CreatedAt = new BsonDateTime(DateTime.UtcNow.AddSeconds(86400)),
                    Role = RoleOptions.Normal,
                    User = firstId,
                };
            });
            result.Start();

            accessMock.Setup(a => a.FindOne(It.IsAny<FilterDefinition<AccessTokenSchema>>()))
                .Returns(result)
                .Verifiable();

            var personMock = new Mock<IRepository<PersonSchema>>();
            var personResult = new Task<PersonSchema>(() =>
            {
                return new PersonSchema()
                {
                    InternalId = secondId,
                };
            });
            personResult.Start();

            personMock.Setup(p => p.FindById(It.IsAny<string>()))
                .Returns(personResult)
                .Verifiable("A person is never searched for.");

            var client = CustomHost.Create(accessMock: accessMock, personMock: personMock);
            var request = CustomRequest.Create(HttpMethod.Delete, "/users/1234567890987654321/");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.Unauthorized,
                "Deleting a user is not unauthorized when it is not the same.");
        }

        [Fact]
        public async Task AdminIsAllowsAuthorizedDeleteUser()
        {
            var firstId = ObjectId.GenerateNewId();
            var secondId = ObjectId.GenerateNewId();

            var accessMock = new Mock<IRepository<AccessTokenSchema>>();
            var result = new Task<AccessTokenSchema>(() =>
            {
                return new AccessTokenSchema
                {
                    CreatedAt = new BsonDateTime(DateTime.UtcNow.AddSeconds(86400)),
                    Role = RoleOptions.Admin,
                    User = firstId,
                };
            });
            result.Start();

            accessMock.Setup(a => a.FindOne(It.IsAny<FilterDefinition<AccessTokenSchema>>()))
                .Returns(result)
                .Verifiable();

            var personMock = new Mock<IRepository<PersonSchema>>();
            var personResult = new Task<PersonSchema>(() =>
            {
                return new PersonSchema()
                {
                    InternalId = secondId,
                };
            });
            personResult.Start();

            personMock.Setup(p => p.FindById(It.IsAny<string>()))
                .Returns(personResult)
                .Verifiable("A person is never searched for.");

            var client = CustomHost.Create(accessMock: accessMock, personMock: personMock);
            var request = CustomRequest.Create(HttpMethod.Delete, "/users/1234567890987654321/");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ResponceIsOkGetUser()
        {
            var accessMock = new Mock<IRepository<AccessTokenSchema>>();
            var result = new Task<AccessTokenSchema>(() =>
            {
                return new AccessTokenSchema
                {
                    CreatedAt = new BsonDateTime(DateTime.UtcNow.AddSeconds(86400)),
                    Role = RoleOptions.Admin,
                };
            });
            result.Start();

            accessMock.Setup(a => a.FindOne(It.IsAny<FilterDefinition<AccessTokenSchema>>()))
                .Returns(result)
                .Verifiable();

            var personMock = new Mock<IRepository<PersonSchema>>();
            var personResult = new Task<PersonSchema>(() =>
            {
                return new PersonSchema()
                {
                };
            });
            personResult.Start();

            personMock.Setup(p => p.FindById(It.IsAny<string>()))
                .Returns(personResult)
                .Verifiable("A person is never searched for.");

            var client = CustomHost.Create(accessMock: accessMock, personMock: personMock);
            var request = CustomRequest.Create(HttpMethod.Delete, "/users/1234567890987654321/");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.True(body == "OK", "Deleting a user successfully does not have the body of 'OK'.");
        }

        [Fact]
        public async Task UpdateIsCalledGetUser()
        {
            var accessMock = new Mock<IRepository<AccessTokenSchema>>();
            var result = new Task<AccessTokenSchema>(() =>
            {
                return new AccessTokenSchema
                {
                    CreatedAt = new BsonDateTime(DateTime.UtcNow.AddSeconds(86400)),
                    Role = RoleOptions.Admin,
                };
            });
            result.Start();

            accessMock.Setup(a => a.FindOne(It.IsAny<FilterDefinition<AccessTokenSchema>>()))
                .Returns(result)
                .Verifiable();

            var personMock = new Mock<IRepository<PersonSchema>>();
            var personResult = new Task<PersonSchema>(() =>
            {
                return new PersonSchema()
                {
                };
            });
            personResult.Start();

            personMock.Setup(p => p.FindById(It.IsAny<string>()))
                .Returns(personResult)
                .Verifiable("A person is never searched for.");

            personMock.Setup(p => p.Update(
                It.IsAny<FilterDefinition<PersonSchema>>(),
                It.IsAny<UpdateDefinition<PersonSchema>>()))
                .Verifiable("A person is never updated.");

            var client = CustomHost.Create(accessMock: accessMock, personMock: personMock);
            var request = CustomRequest.Create(HttpMethod.Delete, "/users/1234567890987654321/");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task RequiresAccessPostSignUp()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Post, "/authentication/signUps", false);
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Creating a sign-up token is a not bad request without access token.");
        }

        [Fact]
        public async Task AddIsCalledPostSignUp()
        {
            var accessMock = new Mock<IRepository<AccessTokenSchema>>();

            var result = new Task<AccessTokenSchema>(() =>
            {
                return new AccessTokenSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Hash = string.Empty,
                    User = ObjectId.GenerateNewId(),

                    // A day so the token can't expire while running
                    CreatedAt = new BsonDateTime(DateTime.UtcNow.AddSeconds(86400)),
                    Role = RoleOptions.Admin,
                };
            });
            result.Start();

            accessMock.Setup(a => a.FindOne(It.IsAny<FilterDefinition<AccessTokenSchema>>()))
                .Returns(result);

            accessMock.Setup(s => s.Add(It.IsAny<AccessTokenSchema>()))
                .Verifiable("An access token is never added.");

            var client = CustomHost.Create(accessMock: accessMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/authentication/signUps");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ResponceIsValidPostSignUp()
        {
            var accessMock = new Mock<IRepository<AccessTokenSchema>>();
            var result = new Task<AccessTokenSchema>(() =>
            {
                return new AccessTokenSchema
                {
                    CreatedAt = new BsonDateTime(DateTime.UtcNow),
                    Role = RoleOptions.Admin,
                };
            });
            result.Start();

            accessMock.Setup(a => a.FindOne(It.IsAny<FilterDefinition<AccessTokenSchema>>()))
                .Returns(result)
                .Verifiable();

            var client = CustomHost.Create(accessMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/authentication/signUps");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var data = BsonSerializer.Deserialize<SignUpTokenResponce>(body);

            Assert.False(data == null, "SignUp data is null.");
            Assert.False(data.SignUpToken == null, "SignUp token data is null.");
            Assert.False(data.SignUpToken == string.Empty, "SignUp token data is empty.");
            Assert.False(data.Expiration == null, "SignUp expiration data is null.");
        }

        [Theory]
        [InlineData("?")]
        [InlineData("/?")]
        [InlineData("/?sdfsdf=sdfdsf34543&")]
        public async Task ResponceIsValidBsonPostSignUp(string input)
        {
            var accessMock = new Mock<IRepository<AccessTokenSchema>>();
            var result = new Task<AccessTokenSchema>(() =>
            {
                return new AccessTokenSchema
                {
                    CreatedAt = new BsonDateTime(DateTime.UtcNow),
                    Role = RoleOptions.Admin,
                };
            });
            result.Start();

            accessMock.Setup(a => a.FindOne(It.IsAny<FilterDefinition<AccessTokenSchema>>()))
                .Returns(result)
                .Verifiable();

            var client = CustomHost.Create(accessMock);

            var request = CustomRequest.Create(HttpMethod.Post, $"/authentication/signUps{input}bson=true");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsByteArrayAsync();
            var data = BsonSerializer.Deserialize<SignUpTokenResponce>(body);

            Assert.False(data == null, "SignUp data is null.");
            Assert.False(data.SignUpToken == null, "SignUp token data is null.");
            Assert.False(data.SignUpToken == string.Empty, "SignUp token data is empty.");
            Assert.False(data.Expiration == null, "SignUp expiration data is null.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("/")]
        [InlineData("?")]
        [InlineData("/?")]
        [InlineData("/?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task MultipleRoutesPostSignUp(string input)
        {
            var accessMock = new Mock<IRepository<AccessTokenSchema>>();
            var result = new Task<AccessTokenSchema>(() =>
            {
                return new AccessTokenSchema
                {
                    CreatedAt = new BsonDateTime(DateTime.UtcNow),
                    Role = RoleOptions.Admin,
                };
            });
            result.Start();

            accessMock.Setup(a => a.FindOne(It.IsAny<FilterDefinition<AccessTokenSchema>>()))
                .Returns(result)
                .Verifiable();

            var client = CustomHost.Create(accessMock);

            var request = CustomRequest.Create(HttpMethod.Post, $"/authentication/signUps{input}");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ResponceExpirationIsInTheFuturePostSignUp()
        {
            var now = new BsonDateTime(DateTime.UtcNow);

            var accessMock = new Mock<IRepository<AccessTokenSchema>>();
            var result = new Task<AccessTokenSchema>(() =>
            {
                return new AccessTokenSchema
                {
                    CreatedAt = new BsonDateTime(DateTime.UtcNow),
                    Role = RoleOptions.Admin,
                };
            });
            result.Start();

            accessMock.Setup(a => a.FindOne(It.IsAny<FilterDefinition<AccessTokenSchema>>()))
                .Returns(result)
                .Verifiable();

            var client = CustomHost.Create(accessMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/authentication/signUps");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            var data = BsonSerializer.Deserialize<SignUpTokenResponce>(body);

            Assert.True(data.Expiration > now, "SignUp expiration data is before the current time.");
        }

        [Fact]
        public async Task ResponceTokensAreDifferentPostSignUp()
        {
            var accessMock = new Mock<IRepository<AccessTokenSchema>>();
            var result = new Task<AccessTokenSchema>(() =>
            {
                return new AccessTokenSchema
                {
                    CreatedAt = new BsonDateTime(DateTime.UtcNow),
                    Role = RoleOptions.Admin,
                };
            });
            result.Start();

            accessMock.Setup(a => a.FindOne(It.IsAny<FilterDefinition<AccessTokenSchema>>()))
                .Returns(result)
                .Verifiable();

            var client = CustomHost.Create(accessMock);

            var requestFirst = CustomRequest.Create(HttpMethod.Post, "/authentication/signUps");
            var responseFirst = await client.SendAsync(requestFirst);
            responseFirst.EnsureSuccessStatusCode();

            var bodyFirst = await responseFirst.Content.ReadAsStringAsync();

            var dataFirst = BsonSerializer.Deserialize<SignUpTokenResponce>(bodyFirst);

            var requestSecond = CustomRequest.Create(HttpMethod.Post, "/authentication/signUps");
            var responseSecond = await client.SendAsync(requestSecond);
            responseFirst.EnsureSuccessStatusCode();

            var bodySecond = await responseSecond.Content.ReadAsStringAsync();

            var dataSecond = BsonSerializer.Deserialize<SignUpTokenResponce>(bodySecond);

            Assert.True(dataFirst.SignUpToken != dataSecond.SignUpToken, "SignUp tokens are the same.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("/")]
        [InlineData("?")]
        [InlineData("/?")]
        [InlineData("/?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task OtherMethodsSignUp(string input)
        {
            var client = CustomHost.Create();

            var requestPut = CustomRequest.Create(HttpMethod.Put, $"/authentication/signUps{input}");
            var responsePut = await client.SendAsync(requestPut);

            Assert.True(
                responsePut.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Putting SignUp is an allowed method.");

            var requestPatch = CustomRequest.Create(HttpMethod.Patch, $"/authentication/signUps{input}");
            var responsePatch = await client.SendAsync(requestPatch);

            Assert.True(
                responsePatch.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Patching SignUp is an allowed method.");

            var requestGet = CustomRequest.Create(HttpMethod.Get, $"/authentication/signUps{input}");
            var responseGet = await client.SendAsync(requestGet);

            Assert.True(
                responseGet.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Gettings SignUp is an allowed method.");

            var requestDelete = CustomRequest.Create(HttpMethod.Delete, $"/authentication/signUps{input}");
            var responseDelete = await client.SendAsync(requestDelete);

            Assert.True(
                responseDelete.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Deleting SignUp is an allowed method.");
        }
    }
}