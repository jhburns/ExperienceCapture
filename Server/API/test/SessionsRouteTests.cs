namespace Carter.Tests.Route.PreSecurity
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Carter.App.Lib.Repository;
    using Carter.App.Route.Sessions;
    using Carter.App.Route.Users;

    using Carter.Tests.HostingExtra;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;

    using Moq;

    using Xunit;

    public class SessionsTests
    {
        [Fact]
        public async Task RequiresAccessPostSessions()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions", false);
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Creating a new session is a bad request without access token.");
        }

        [Fact]
        public async Task ChecksIdManyPostSessions()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var nullResult = new Task<SessionSchema>(() =>
            {
                return null;
            });
            nullResult.Start();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = null,
                    Id = null,
                    User = null,
                    CreatedAt = null,
                    Tags = null,
                    ExportState = ExportOptions.Done,
                };
            });
            result.Start();

            sessionMock.SetupSequence(a => a.FindById(It.IsAny<string>()))
                .Returns(result)
                .Returns(result)
                .Returns(result)
                .Returns(result)
                .Returns(result)
                .Returns(result)
                .Returns(result)
                .Returns(result)
                .Returns(result)
                .Returns(result)
                .Returns(result)
                .Returns(nullResult);

            var personMock = new Mock<IRepository<PersonSchema>>();

            var personResult = new Task<PersonSchema>(() =>
            {
                return new PersonSchema
                {
                    InternalId = null,
                    Id = null,
                    Fullname = null,
                    Firstname = null,
                    Lastname = null,
                    Email = null,
                    CreatedAt = null,
                };
            });
            personResult.Start();

            personMock.Setup(p => p.FindOne(It.IsAny<FilterDefinition<PersonSchema>>()))
                .Returns(personResult)
                .Verifiable("A person was never searched for");

            var client = CustomHost.Create(sessionMock: sessionMock, personMock: personMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions");
            var response = await client.SendAsync(request);
        }

        [Fact]
        public async Task ChecksIdOnePostSessions()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return null;
            });
            result.Start();

            sessionMock.Setup(a => a.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var personMock = new Mock<IRepository<PersonSchema>>();

            var personResult = new Task<PersonSchema>(() =>
            {
                return new PersonSchema
                {
                    InternalId = null,
                    Id = null,
                    Fullname = null,
                    Firstname = null,
                    Lastname = null,
                    Email = null,
                    CreatedAt = null,
                };
            });
            personResult.Start();

            personMock.Setup(p => p.FindOne(It.IsAny<FilterDefinition<PersonSchema>>()))
                .Returns(personResult)
                .Verifiable("A person was never searched for");

            var client = CustomHost.Create(sessionMock: sessionMock, personMock: personMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions");
            var response = await client.SendAsync(request);
        }

        [Fact]
        public async Task AccessTokenIsSearchedSessions()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return null;
            });
            result.Start();

            sessionMock.Setup(a => a.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var accessMock = new Mock<IRepository<AccessTokenSchema>>();

            var accessResult = new Task<AccessTokenSchema>(() =>
            {
                return null;
            });
            accessResult.Start();

            accessMock.Setup(a => a.FindOne(It.IsAny<FilterDefinition<AccessTokenSchema>>()))
                .Returns(accessResult)
                .Verifiable("An access token was never searched for.");

            var personMock = new Mock<IRepository<PersonSchema>>();

            var personResult = new Task<PersonSchema>(() =>
            {
                return new PersonSchema
                {
                    InternalId = null,
                    Id = null,
                    Fullname = null,
                    Firstname = null,
                    Lastname = null,
                    Email = null,
                    CreatedAt = null,
                };
            });
            personResult.Start();

            personMock.Setup(p => p.FindOne(It.IsAny<FilterDefinition<PersonSchema>>()))
                .Returns(personResult)
                .Verifiable("A person was never searched for");

            var client = CustomHost.Create(sessionMock: sessionMock, personMock: personMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions");
            var response = await client.SendAsync(request);
        }

        [Fact]
        public async Task NewSessionIsAddedPostSessions()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return null;
            });
            result.Start();

            sessionMock.Setup(a => a.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            sessionMock.Setup(s => s.Add(It.IsAny<SessionSchema>()))
                .Verifiable();

            var personMock = new Mock<IRepository<PersonSchema>>();

            var personResult = new Task<PersonSchema>(() =>
            {
                return new PersonSchema
                {
                    InternalId = null,
                    Id = null,
                    Fullname = null,
                    Firstname = null,
                    Lastname = null,
                    Email = null,
                    CreatedAt = null,
                };
            });
            personResult.Start();

            personMock.Setup(p => p.FindOne(It.IsAny<FilterDefinition<PersonSchema>>()))
                .Returns(personResult)
                .Verifiable("A person was never searched for");

            var client = CustomHost.Create(sessionMock: sessionMock, personMock: personMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions");
            var response = await client.SendAsync(request);
        }

        [Fact]
        public async Task NewCaptureCollectionIsIndexedPostSessions()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return null;
            });
            result.Start();

            sessionMock.Setup(a => a.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var personMock = new Mock<IRepository<PersonSchema>>();

            var personResult = new Task<PersonSchema>(() =>
            {
                return new PersonSchema
                {
                    InternalId = null,
                    Id = null,
                    Fullname = null,
                    Firstname = null,
                    Lastname = null,
                    Email = null,
                    CreatedAt = null,
                };
            });
            personResult.Start();

            personMock.Setup(p => p.FindOne(It.IsAny<FilterDefinition<PersonSchema>>()))
                .Returns(personResult)
                .Verifiable("A person was never searched for");

            var captureMock = new Mock<IRepository<BsonDocument>>();

            captureMock.Setup(a => a.Configure(It.IsAny<string>()))
                .Verifiable("A capture collection was never configured.");

            captureMock.Setup(a => a.Index(It.IsAny<IndexKeysDefinition<BsonDocument>>(), null))
                .Verifiable("A capture collection was never indexed.");

            var client = CustomHost.Create(sessionMock: sessionMock, personMock: personMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions");
            var response = await client.SendAsync(request);
        }

        [Theory]
        [InlineData("")]
        [InlineData("/")]
        [InlineData("?")]
        [InlineData("/?")]
        [InlineData("/?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task ResponceIsValidPostSessions(string input)
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return null;
            });
            result.Start();

            sessionMock.Setup(a => a.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var personMock = new Mock<IRepository<PersonSchema>>();

            var personResult = new Task<PersonSchema>(() =>
            {
                return new PersonSchema
                {
                    InternalId = null,
                    Id = null,
                    Fullname = null,
                    Firstname = null,
                    Lastname = null,
                    Email = null,
                    CreatedAt = null,
                };
            });
            personResult.Start();

            personMock.Setup(p => p.FindOne(It.IsAny<FilterDefinition<PersonSchema>>()))
                .Returns(personResult)
                .Verifiable("A person was never searched for");

            var client = CustomHost.Create(sessionMock: sessionMock, personMock: personMock);

            var request = CustomRequest.Create(HttpMethod.Post, $"/sessions{input}");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var data = BsonSerializer.Deserialize<SessionSchema>(body);

            Assert.False(data == null, "New session data is null.");
            Assert.False(data.CreatedAt == null, "New session created at data is null.");
            Assert.False(data.Tags == null, "New session tags data is empty.");
            Assert.False(data.Id == null, "New session id data is null.");
            Assert.False(data.Id == string.Empty, "New session id data is empty.");
            Assert.False(data.User == null, "New session user id data is null.");
            Assert.True(data.IsOngoing, "New session isOngoing data is not true.");
            Assert.True(data.IsOpen, "New session isOngoing data is not true.");
            Assert.True(data.InternalId == null, "New session internal id data is not null.");
            Assert.True(data.User.InternalId == null, "New session user internal id data is not null.");
        }

        [Theory]
        [InlineData("?")]
        [InlineData("?test=sdkfjsdlfksdf&blak=sdfsfds&")]
        public async Task ResponceIsValidBsonSessions(string input)
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return null;
            });
            result.Start();

            sessionMock.Setup(a => a.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var personMock = new Mock<IRepository<PersonSchema>>();

            var personResult = new Task<PersonSchema>(() =>
            {
                return new PersonSchema
                {
                    InternalId = null,
                    Id = null,
                    Fullname = null,
                    Firstname = null,
                    Lastname = null,
                    Email = null,
                    CreatedAt = null,
                };
            });
            personResult.Start();

            personMock.Setup(p => p.FindOne(It.IsAny<FilterDefinition<PersonSchema>>()))
                .Returns(personResult)
                .Verifiable("A person was never searched for");

            var client = CustomHost.Create(sessionMock: sessionMock, personMock: personMock);

            var request = CustomRequest.Create(HttpMethod.Post, $"/sessions{input}bson=true");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsByteArrayAsync();
            var data = BsonSerializer.Deserialize<SessionSchema>(body);

            Assert.False(data == null, "New session data is null.");
            Assert.False(data.CreatedAt == null, "New session created at data is null.");
            Assert.False(data.Tags == null, "New session tags data is empty.");
            Assert.False(data.Id == null, "New session id data is null.");
            Assert.False(data.Id == string.Empty, "New session id data is empty.");
            Assert.False(data.User == null, "New session user id data is null.");
            Assert.True(data.IsOngoing, "New session isOngoing data is not true.");
            Assert.True(data.IsOpen, "New session isOngoing data is not true.");
            Assert.True(data.InternalId == null, "New session internal id data is not null.");
            Assert.True(data.User.InternalId == null, "New session user internal id data is not null.");
        }

        [Fact]
        public async Task UniqueIdsPostSessions()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return null;
            });
            result.Start();

            sessionMock.Setup(a => a.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var personMock = new Mock<IRepository<PersonSchema>>();

            var personResult = new Task<PersonSchema>(() =>
            {
                return new PersonSchema
                {
                    InternalId = null,
                    Id = null,
                    Fullname = null,
                    Firstname = null,
                    Lastname = null,
                    Email = null,
                    CreatedAt = null,
                };
            });
            personResult.Start();

            personMock.Setup(p => p.FindOne(It.IsAny<FilterDefinition<PersonSchema>>()))
                .Returns(personResult)
                .Verifiable("A person was never searched for");

            var client = CustomHost.Create(sessionMock: sessionMock, personMock: personMock);

            var requestFirst = CustomRequest.Create(HttpMethod.Post, "/sessions");
            var responseFirst = await client.SendAsync(requestFirst);

            responseFirst.EnsureSuccessStatusCode();

            var bodyFirst = await responseFirst.Content.ReadAsStringAsync();
            var dataFirst = BsonSerializer.Deserialize<SessionSchema>(bodyFirst);

            var requestSecond = CustomRequest.Create(HttpMethod.Post, "/sessions");
            var responseSecond = await client.SendAsync(requestSecond);

            responseSecond.EnsureSuccessStatusCode();

            var bodySecond = await responseSecond.Content.ReadAsStringAsync();
            var dataSecond = BsonSerializer.Deserialize<SessionSchema>(bodySecond);

            Assert.True(
                dataFirst.Id != dataSecond.Id,
                "Different post sessions responces have the same id");
        }

        [Fact]
        public async Task RequiresAccessGetSessions()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Get, "/sessions", false);
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Getting sessions is a bad request without access token.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("/")]
        [InlineData("?")]
        [InlineData("/?")]
        [InlineData("/?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task OtherMethodsSessions(string input)
        {
            var client = CustomHost.Create();

            var requestPut = CustomRequest.Create(HttpMethod.Put, $"/sessions{input}");
            var responsePut = await client.SendAsync(requestPut);

            Assert.True(
                responsePut.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Putting sessions is an allowed method.");

            var requestPatch = CustomRequest.Create(HttpMethod.Patch, $"/sessions{input}");
            var responsePatch = await client.SendAsync(requestPatch);

            Assert.True(
                responsePatch.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Patching sessions is an allowed method.");

            var requestDelete = CustomRequest.Create(HttpMethod.Delete, $"/sessions{input}");
            var responseDelete = await client.SendAsync(requestDelete);

            Assert.True(
                responseDelete.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Deleting sessions is an allowed method.");
        }
    }
}