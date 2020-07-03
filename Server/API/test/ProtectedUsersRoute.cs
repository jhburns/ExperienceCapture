namespace Carter.Tests.Route.ProtectedUsers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Carter.App.Lib.Repository;
    using Carter.App.Route.ProtectedUsers;
    using Carter.App.Route.Users;

    using Carter.Tests.HostingExtra;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;

    using Moq;

    using Xunit;

    public class ProtectedUsersRoute
    {
        [Fact]
        public async Task RequiresAccessGetUsers()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Get, "/users", false);
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Getting users is not a bad request without access token.");
        }

        [Fact]
        public async Task RequiresAdminRoleGetUsers()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Get, "/users");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.Unauthorized,
                "Getting users without an admin role is not a bad request.");
        }

        [Fact]
        public async Task ResponceIsValidGetUsers()
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

            var personMock = new Mock<IRepository<PersonSchema>>();
            var personResult = new Task<IList<PersonSchema>>(() =>
            {
                return new List<PersonSchema>()
                {
                    new PersonSchema()
                    {
                        InternalId = ObjectId.GenerateNewId(),
                        Id = "1234",
                        Fullname = "Smitty Jensens",
                        Firstname = "Smitty",
                        Lastname = "Jensens",
                        Email = "jensens@gmail.com",
                        CreatedAt = new BsonDateTime(DateTime.UtcNow),
                        Role = RoleOptions.Normal,
                    },
                };
            });
            personResult.Start();

            personMock.Setup(p => p.FindAll(
                It.IsAny<FilterDefinition<PersonSchema>>(),
                It.IsAny<SortDefinition<PersonSchema>>(),
                It.IsAny<int>()))
                .Returns(personResult)
                .Verifiable("Persons are never searched for.");

            var client = CustomHost.Create(accessMock: accessMock, personMock: personMock);
            var request = CustomRequest.Create(HttpMethod.Get, "/users");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var data = BsonSerializer.Deserialize<PersonsResponce>(body);

            Assert.True(data.ContentList.Count == 1, "Get users does not return given array.");

            // TODO: Add this test in more places where it is relevant
            Assert.True(data.ContentList[0].InternalId == null, "Get users does null the internal id field.");
        }

        [Fact]
        public async Task ResponceIsValidBsonGetUsers()
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

            var personMock = new Mock<IRepository<PersonSchema>>();
            var personResult = new Task<IList<PersonSchema>>(() =>
            {
                return new List<PersonSchema>()
                {
                    new PersonSchema()
                    {
                        InternalId = ObjectId.GenerateNewId(),
                        Id = "1234",
                        Fullname = "Smitty Jensens",
                        Firstname = "Smitty",
                        Lastname = "Jensens",
                        Email = "jensens@gmail.com",
                        CreatedAt = new BsonDateTime(DateTime.UtcNow),
                        Role = RoleOptions.Normal,
                    },
                };
            });
            personResult.Start();

            personMock.Setup(p => p.FindAll(
                It.IsAny<FilterDefinition<PersonSchema>>(),
                It.IsAny<SortDefinition<PersonSchema>>(),
                It.IsAny<int>()))
                .Returns(personResult)
                .Verifiable("Persons are never searched for.");

            var client = CustomHost.Create(accessMock: accessMock, personMock: personMock);
            var request = CustomRequest.Create(HttpMethod.Get, "/users?bson=true");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsByteArrayAsync();
            var data = BsonSerializer.Deserialize<PersonsResponce>(body);

            Assert.True(data.ContentList.Count == 1, "Get users does not return given array.");

            // TODO: Add this test in more places where it is relevant
            Assert.True(data.ContentList[0].InternalId == null, "Get users does null the internal id field.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("/")]
        [InlineData("?")]
        [InlineData("/?")]
        [InlineData("/?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task MultipleRoutesGetUsers(string input)
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

            var personMock = new Mock<IRepository<PersonSchema>>();
            var personResult = new Task<IList<PersonSchema>>(() =>
            {
                return new List<PersonSchema>()
                {
                };
            });
            personResult.Start();

            personMock.Setup(p => p.FindAll(
                It.IsAny<FilterDefinition<PersonSchema>>(),
                It.IsAny<SortDefinition<PersonSchema>>(),
                It.IsAny<int>()))
                .Returns(personResult)
                .Verifiable("Persons are never searched for.");

            var client = CustomHost.Create(accessMock: accessMock, personMock: personMock);
            var request = CustomRequest.Create(HttpMethod.Get, $"/users{input}");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task RequiresAccessPostSignUp()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Post, "/authorization/signUp", false);
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Creating a sign-up token is a not bad request without access token.");
        }

        [Fact]
        public async Task RequiresAdminRolePostUsers()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Post, "/authorization/signUp");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.Unauthorized,
                "Creating a sign-up token without an admin role is not a bad request.");
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

            var request = CustomRequest.Create(HttpMethod.Post, "/authorization/signUp");
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

            var request = CustomRequest.Create(HttpMethod.Post, "/authorization/signUp");
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

            var request = CustomRequest.Create(HttpMethod.Post, $"/authorization/signUp{input}bson=true");
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

            var request = CustomRequest.Create(HttpMethod.Post, $"/authorization/signUp{input}");
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

            var request = CustomRequest.Create(HttpMethod.Post, "/authorization/signUp");
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

            var requestFirst = CustomRequest.Create(HttpMethod.Post, "/authorization/signUp");
            var responseFirst = await client.SendAsync(requestFirst);
            responseFirst.EnsureSuccessStatusCode();

            var bodyFirst = await responseFirst.Content.ReadAsStringAsync();

            var dataFirst = BsonSerializer.Deserialize<SignUpTokenResponce>(bodyFirst);

            var requestSecond = CustomRequest.Create(HttpMethod.Post, "/authorization/signUp");
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

            var requestPut = CustomRequest.Create(HttpMethod.Put, $"/authorization/signUp{input}");
            var responsePut = await client.SendAsync(requestPut);

            Assert.True(
                responsePut.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Putting SignUp is an allowed method.");

            var requestPatch = CustomRequest.Create(HttpMethod.Patch, $"/authorization/signUp{input}");
            var responsePatch = await client.SendAsync(requestPatch);

            Assert.True(
                responsePatch.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Patching SignUp is an allowed method.");

            var requestGet = CustomRequest.Create(HttpMethod.Get, $"/authorization/signUp{input}");
            var responseGet = await client.SendAsync(requestGet);

            Assert.True(
                responseGet.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Gettings SignUp is an allowed method.");

            var requestDelete = CustomRequest.Create(HttpMethod.Delete, $"/authorization/signUp{input}");
            var responseDelete = await client.SendAsync(requestDelete);

            Assert.True(
                responseDelete.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Deleting SignUp is an allowed method.");
        }
    }
}