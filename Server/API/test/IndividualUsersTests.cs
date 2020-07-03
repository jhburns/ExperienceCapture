namespace Carter.Tests.Route.ProtectedUsers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Carter.App.Lib.Repository;
    using Carter.App.Route.Users;

    using Carter.Tests.HostingExtra;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;

    using Moq;

    using Xunit;

    public class IndividualUsersRoute
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
    }
}