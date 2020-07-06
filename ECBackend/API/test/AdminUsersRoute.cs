namespace Carter.Tests.Route.AdminUserRoutes
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Carter.App.Lib.Repository;
    using Carter.App.Route.AdminUsers;
    using Carter.App.Route.ProtectedUsersAndAuthentication;
    using Carter.App.Route.UsersAndAuthentication;

    using Carter.Tests.HostingExtra;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;

    using Moq;

    using Xunit;

    public class AdminUserTests
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
    }
}