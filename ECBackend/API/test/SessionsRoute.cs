namespace Carter.Tests.Route.Sessions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Carter.App.Lib.Repository;
    using Carter.App.Lib.Timer;
    using Carter.App.Route.Sessions;
    using Carter.App.Route.UsersAndAuthentication;

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

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
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

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
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

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
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

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
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

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
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

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
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
            Assert.True(data.ExportState == ExportOptions.NotStarted, "New session export state data is not 'not started'.");
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

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
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

        [Fact]
        public async Task EmptyListIsAllowedGetSessions()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<IList<SessionSchema>>(() =>
            {
                return new List<SessionSchema>();
            });
            result.Start();

            sessionMock.Setup(s => s.FindAll(
                    It.IsAny<FilterDefinition<SessionSchema>>(),
                    It.IsAny<SortDefinition<SessionSchema>>(),
                    It.IsAny<int>()))
                .Returns(result)
                .Verifiable("Sessions are never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Get, "/sessions");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("")]
        [InlineData("/")]
        [InlineData("?")]
        public async Task ResponceIsValidGetSessions(string input)
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<IList<SessionSchema>>(() =>
            {
                return new List<SessionSchema>();
            });
            result.Start();

            sessionMock.Setup(s => s.FindAll(
                    It.IsAny<FilterDefinition<SessionSchema>>(),
                    It.IsAny<SortDefinition<SessionSchema>>(),
                    It.IsAny<int>()))
                .Returns(result)
                .Verifiable("Sessions are never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Get, $"/sessions{input}");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var data = BsonSerializer.Deserialize<SessionsResponce>(body);

            Assert.True(data.ContentList.Count == 0, "Get sessions does not return given array.");
        }

        [Theory]
        [InlineData("?bson=true")]
        [InlineData("?sdfdsff=534rwfere&bson=true")]
        public async Task ResponceIsValidBsonGetSessions(string input)
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<IList<SessionSchema>>(() =>
            {
                return new List<SessionSchema>();
            });
            result.Start();

            sessionMock.Setup(s => s.FindAll(
                    It.IsAny<FilterDefinition<SessionSchema>>(),
                    It.IsAny<SortDefinition<SessionSchema>>(),
                    It.IsAny<int>()))
                .Returns(result)
                .Verifiable("Sessions are never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Get, $"/sessions{input}");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsByteArrayAsync();
            var data = BsonSerializer.Deserialize<SessionsResponce>(body);

            Assert.True(data.ContentList.Count == 0, "Get sessions does not return given list.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(1000)]
        public async Task ResponceIsCorrectForAllGetSessions(int input)
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<IList<SessionSchema>>(() =>
            {
                return Enumerable.Repeat(
                    new SessionSchema
                    {
                        InternalId = ObjectId.GenerateNewId(),
                        Id = null,
                        User = new PersonSchema()
                        {
                            InternalId = ObjectId.GenerateNewId(),
                        },
                        CreatedAt = null,
                        Tags = null,
                        ExportState = ExportOptions.Done,
                    },
                    input)
                    .ToList();
            });
            result.Start();

            sessionMock.Setup(s => s.FindAll(
                    It.IsAny<FilterDefinition<SessionSchema>>(),
                    It.IsAny<SortDefinition<SessionSchema>>(),
                    It.IsAny<int>()))
                .Returns(result)
                .Verifiable("Sessions are never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Get, $"/sessions");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var data = BsonSerializer.Deserialize<SessionsResponce>(body);

            Assert.True(data.ContentList.Count == input, "Get sessions does not return given list length.");

            foreach (var session in data.ContentList)
            {
                Assert.True(session.InternalId == null, "A session does not have a null id.");
                Assert.True(session.User.InternalId == null, "A session's user does not have a null id.");
            }
        }

        [Fact]
        public async Task IsNotOngoingWhenClosedGetSessions()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<IList<SessionSchema>>(() =>
            {
                return new List<SessionSchema>()
                {
                    new SessionSchema
                    {
                        User = new PersonSchema(),
                        IsOpen = false,
                    },
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindAll(
                    It.IsAny<FilterDefinition<SessionSchema>>(),
                    It.IsAny<SortDefinition<SessionSchema>>(),
                    It.IsAny<int>()))
                .Returns(result)
                .Verifiable("Sessions are never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Get, $"/sessions");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var data = BsonSerializer.Deserialize<SessionsResponce>(body);

            Assert.True(data.ContentList[0].IsOngoing == false, "isOngoing is not false when isOpen is false.");
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(-1, true)]
        [InlineData(-1000, true)]
        [InlineData(1, true)]
        [InlineData(100, true)]
        [InlineData(300, false)]
        [InlineData(10000, false)]
        public async Task ChecksWhenNotStartedGetSessions(int seconds, bool isOngoing)
        {
            var setTime = new DateProvider().UtcNow;
            var dateMock = new Mock<IDateExtra>();
            dateMock.SetupGet(d => d.UtcNow)
                .Returns(setTime.AddSeconds(seconds))
                .Verifiable("Now was never called.");

            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<IList<SessionSchema>>(() =>
            {
                return new List<SessionSchema>()
                {
                    new SessionSchema
                    {
                        User = new PersonSchema(),
                        IsOpen = true,
                        CreatedAt = new BsonDateTime(setTime),

                        // Being explicit, so its clear the session doesn't have any captures yet
                        LastCaptureAt = null,
                    },
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindAll(
                    It.IsAny<FilterDefinition<SessionSchema>>(),
                    It.IsAny<SortDefinition<SessionSchema>>(),
                    It.IsAny<int>()))
                .Returns(result)
                .Verifiable("Sessions are never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock, dateMock: dateMock);

            var request = CustomRequest.Create(HttpMethod.Get, $"/sessions");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var data = BsonSerializer.Deserialize<SessionsResponce>(body);

            Assert.True(data.ContentList[0].IsOngoing == isOngoing, "isOngoing is not aligned with given value.");
        }

        [Theory]
        [InlineData(0, 0, true)]
        [InlineData(-1, 0, true)]
        [InlineData(0, -1, true)]
        [InlineData(-1, -1, true)]
        [InlineData(-1000, 0, true)]
        [InlineData(0, -1000, false)]
        [InlineData(-1000, -1000, true)]
        [InlineData(0, 1, true)]
        [InlineData(0, 6, true)]
        [InlineData(0, 1000, true)]
        [InlineData(1, 1000, true)]
        [InlineData(6, 1000, true)]
        [InlineData(1000, 1000, true)]
        [InlineData(1, 0, true)]
        [InlineData(6, 0, false)]
        [InlineData(1000, 0, false)]
        public async Task ChecksWhenStartedGetSessions(int currentSeconds, int captureSecond, bool isOngoing)
        {
            var setTime = new DateProvider().UtcNow;
            var dateMock = new Mock<IDateExtra>();
            dateMock.SetupGet(d => d.UtcNow)
                .Returns(setTime.AddSeconds(currentSeconds))
                .Verifiable("Now was never called.");

            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<IList<SessionSchema>>(() =>
            {
                return new List<SessionSchema>()
                {
                    new SessionSchema
                    {
                        User = new PersonSchema(),
                        IsOpen = true,
                        LastCaptureAt = new BsonDateTime(setTime.AddSeconds(captureSecond)),
                    },
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindAll(
                    It.IsAny<FilterDefinition<SessionSchema>>(),
                    It.IsAny<SortDefinition<SessionSchema>>(),
                    It.IsAny<int>()))
                .Returns(result)
                .Verifiable("Sessions are never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock, dateMock: dateMock);

            var request = CustomRequest.Create(HttpMethod.Get, $"/sessions");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var data = BsonSerializer.Deserialize<SessionsResponce>(body);

            Assert.True(data.ContentList[0].IsOngoing == isOngoing, "isOngoing is not aligned with given value.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10000)]
        public async Task ZeroOrNegativePageIsBadGetSessions(int input)
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<IList<SessionSchema>>(() =>
            {
                return new List<SessionSchema>();
            });
            result.Start();

            sessionMock.Setup(s => s.FindAll(
                    It.IsAny<FilterDefinition<SessionSchema>>(),
                    It.IsAny<SortDefinition<SessionSchema>>(),
                    It.IsAny<int>()))
                .Returns(result)
                .Verifiable("Sessions are never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Get, $"/sessions?page={input}");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Gettings sessions with a negative page query is not a bad request.");
        }

        [Theory]
        [InlineData("a")]
        [InlineData("alphabetically")]
        [InlineData("newest")]
        [InlineData("Newest")]
        public async Task NonEnumValueIsBadGetSessions(string input)
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<IList<SessionSchema>>(() =>
            {
                return new List<SessionSchema>();
            });
            result.Start();

            sessionMock.Setup(s => s.FindAll(
                    It.IsAny<FilterDefinition<SessionSchema>>(),
                    It.IsAny<SortDefinition<SessionSchema>>(),
                    It.IsAny<int>()))
                .Returns(result)
                .Verifiable("Sessions are never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Get, $"/sessions?sort={input}");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Gettings sessions with a wrong sort enum value is not a bad request.");
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(10, 1)]
        [InlineData(800, 80)]
        [InlineData(5, 1)]
        [InlineData(11, 2)]
        [InlineData(19, 2)]
        [InlineData(101, 11)]
        public async Task PageTotalIsCalculatedCorrectlyGetSessions(long output, long expectedTotal)
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<IList<SessionSchema>>(() =>
            {
                return new List<SessionSchema>();
            });
            result.Start();

            sessionMock.Setup(s => s.FindAll(
                    It.IsAny<FilterDefinition<SessionSchema>>(),
                    It.IsAny<SortDefinition<SessionSchema>>(),
                    It.IsAny<int>()))
                .Returns(result)
                .Verifiable("Sessions are never searched for.");

            var resultCount = new Task<long>(() =>
            {
                return output;
            });
            resultCount.Start();

            sessionMock.Setup(s => s.FindThenCount(It.IsAny<FilterDefinition<SessionSchema>>()))
                .Returns(resultCount)
                .Verifiable("Sessions counted.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Get, $"/sessions");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var data = BsonSerializer.Deserialize<SessionsResponce>(body);

            Assert.True(data.PageTotal == expectedTotal, "The page total is not calculated correctly.");
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

        [Fact]
        public async Task RequiresAccessPostSession()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions/EXEX", false);
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Adding a capture to a session a bad request without access token.");
        }

        [Fact]
        public async Task SessionIsNotFoundPostSession()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions/EXEX");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.NotFound,
                "Adding a capture to a missing session is not found.");
        }

        [Fact]
        public async Task IsOpenErrorsPostSession()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema()
                {
                    IsOpen = false,
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions/EXEX");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Adding a capture to a closed session is a bad request.");
        }

        [Fact]
        public async Task BsonNotAllowedNormallyPostSession()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema()
                {
                    IsOpen = true,
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions/EXEX");
            var content = new
            {
                frameInfo = new
                {
                    realtimeSinceStartup = 0.1,
                },
            };
            request.Content = new ByteArrayContent(content.ToBson());
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Posting a bson capture without the query parameter is allowed.");
        }

        [Fact]
        public async Task JsonNotAllowedWithParamPostSession()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema()
                {
                    IsOpen = true,
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions/EXEX?bson=true");
            var content = new
            {
                frameInfo = new
                {
                    realtimeSinceStartup = 0.1,
                },
            };
            request.Content = new StringContent(content.ToJson(), Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Posting a json capture with the bson query parameter is allowed.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("{ \"frameInfo\": {} }")]
        [InlineData("{ \"frameInfo\": \"hello\" }")]
        [InlineData("{ \"frameInfo\": { \"test\": 0.1 } }")]
        [InlineData("{ \"frameInfo\": { \"realtimeSinceStartup\": true } }")]
        [InlineData("{ \"frameInfo\": { \"realtimeSinceStartup\": 1 } }")]
        [InlineData("{ \"frameInfo\": { }, \"realtimeSinceStartup\": 0.1 }")]
        public async Task DoesNotAcceptInvalidSchemaPostSession(string input)
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema()
                {
                    IsOpen = true,
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions/EXEX");
            request.Content = new StringContent(input, Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Posting a capture with invalid schema is allowed.");
        }

        [Fact]
        public async Task CallsThingsPostSession()
        {
            var captureMock = new Mock<IRepository<BsonDocument>>();
            captureMock.Setup(a => a.Configure(It.IsAny<string>()))
                .Verifiable("A capture collection was never configured.");

            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema()
                {
                    IsOpen = true,
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            sessionMock.Setup(s => s.Update(
                    It.IsAny<FilterDefinition<SessionSchema>>(),
                    It.IsAny<UpdateDefinition<SessionSchema>>()))
                .Verifiable("An update was never called");

            var client = CustomHost.Create(sessionMock: sessionMock, captureMock: captureMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions/EXEX");
            request.Content = new StringContent("{ \"frameInfo\": { \"realtimeSinceStartup\": 0.1 } }", Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("/EXEX", "{ \"frameInfo\": { \"realtimeSinceStartup\": 0.1 } }")]
        [InlineData("/e", "{ \"frameInfo\": { \"realtimeSinceStartup\": 0.1 } }")]
        [InlineData("/esdfer34wfv34f44wfsd", "{ \"frameInfo\": { \"realtimeSinceStartup\": 0.1 } }")]
        [InlineData("/EXEX/", "{ \"frameInfo\": { \"realtimeSinceStartup\": 0.1 } }")]
        [InlineData("/EXEX?", "{ \"frameInfo\": { \"realtimeSinceStartup\": 0.1 } }")]
        [InlineData("/EXEX/?", "{ \"frameInfo\": { \"realtimeSinceStartup\": 0.1 } }")]
        [InlineData("/EXEX/?s43f4r=34fr&dfwe=s1123", "{ \"frameInfo\": { \"realtimeSinceStartup\": 0.1 } }")]
        [InlineData("/EXEX", "{ \"frameInfo\": { \"realtimeSinceStartup\": 0.1, \"test\": 1234 } }")]
        [InlineData("/EXEX", "{ \"frameInfo\": { \"realtimeSinceStartup\": 0.1}, \"test\": 1234, \"test2\": [1, 2, 3] }")]
        [InlineData("/EXEX", "{ \"gameObjects\": { \"Player\": { \"x\": 1.01 } }, \"frameInfo\": { \"realtimeSinceStartup\": 0.1} }")]
        public async Task AllowsManyDocumentsAndPathsPostSession(string path, string body)
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema()
                {
                    IsOpen = true,
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Post, $"/sessions{path}");
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ResponceIsOkPostSession()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema()
                {
                    IsOpen = true,
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions/EXEX");
            request.Content = new StringContent("{ \"frameInfo\": { \"realtimeSinceStartup\": 0.1 } }", Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            Assert.True(
                body == "OK",
                "The responce body of posting a capture is not 'OK'.");
        }

        [Fact]
        public async Task RequiresAccessGetSession()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Get, "/sessions/EXEX", false);
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Getting a session is a bad request without access token.");
        }

        [Fact]
        public async Task SessionIsNotFoundGetSession()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Get, "/sessions/EXEX");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.NotFound,
                "Getting missing session is not found.");
        }

        [Fact]
        public async Task FindIsCalledGetSession()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema()
                {
                    User = new PersonSchema
                    {
                    },
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Get, "/sessions/EXEX");
            var response = await client.SendAsync(request);
        }

        [Theory]
        [InlineData("")]
        [InlineData("/")]
        [InlineData("?")]
        [InlineData("/?")]
        [InlineData("/?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task ResponceIsValidGetSession(string input)
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    User = new PersonSchema
                    {
                        InternalId = ObjectId.GenerateNewId(),
                    },
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Get, $"/sessions/EXEX{input}");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var data = BsonSerializer.Deserialize<SessionSchema>(body);

            Assert.False(data == null, "Session data is null.");
            Assert.True(data.InternalId == null, "Session id data is not null.");
            Assert.True(data.User.InternalId == null, "Session user is data is not null.");
        }

        [Fact]
        public async Task ResponceIsValidBsonGetSession()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    User = new PersonSchema
                    {
                    },
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Get, $"/sessions/EXEX?bson=true");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsByteArrayAsync();
            var data = BsonSerializer.Deserialize<SessionSchema>(body);
        }

        [Fact]
        public async Task IsNotOngoingWhenClosedGetSession()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema()
                {
                    User = new PersonSchema(),
                    IsOpen = false,
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session is never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Get, $"/sessions/EXEX");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var data = BsonSerializer.Deserialize<SessionSchema>(body);

            Assert.True(data.IsOngoing == false, "IsOngoing is not false when isOpen also is.");
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(-1, true)]
        [InlineData(-1000, true)]
        [InlineData(1, true)]
        [InlineData(100, true)]
        [InlineData(300, false)]
        [InlineData(10000, false)]
        public async Task ChecksWhenNotStartedGetSession(int seconds, bool isOngoing)
        {
            var setTime = new DateProvider().UtcNow;
            var dateMock = new Mock<IDateExtra>();
            dateMock.SetupGet(d => d.UtcNow)
                .Returns(setTime.AddSeconds(seconds))
                .Verifiable("Now was never called.");

            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema()
                {
                    User = new PersonSchema(),
                    IsOpen = true,
                    CreatedAt = new BsonDateTime(setTime),

                    // Being explicit, so its clear the session doesn't have any captures yet
                    LastCaptureAt = null,
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session is never searched for");

            var client = CustomHost.Create(sessionMock: sessionMock, dateMock: dateMock);

            var request = CustomRequest.Create(HttpMethod.Get, $"/sessions/EXEX");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var data = BsonSerializer.Deserialize<SessionSchema>(body);

            Assert.True(data.IsOngoing == isOngoing, "isOngoing is not aligned with given value.");
        }

        [Theory]
        [InlineData(0, 0, true)]
        [InlineData(-1, 0, true)]
        [InlineData(0, -1, true)]
        [InlineData(-1, -1, true)]
        [InlineData(-1000, 0, true)]
        [InlineData(0, -1000, false)]
        [InlineData(-1000, -1000, true)]
        [InlineData(0, 1, true)]
        [InlineData(0, 6, true)]
        [InlineData(0, 1000, true)]
        [InlineData(1, 1000, true)]
        [InlineData(6, 1000, true)]
        [InlineData(1000, 1000, true)]
        [InlineData(1, 0, true)]
        [InlineData(6, 0, false)]
        [InlineData(1000, 0, false)]
        public async Task ChecksWhenStartedGetSession(int currentSeconds, int captureSecond, bool isOngoing)
        {
            var setTime = new DateProvider().UtcNow;
            var dateMock = new Mock<IDateExtra>();
            dateMock.SetupGet(d => d.UtcNow)
                .Returns(setTime.AddSeconds(currentSeconds))
                .Verifiable("Now was never called.");

            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema()
                {
                    User = new PersonSchema(),
                    IsOpen = true,
                    LastCaptureAt = new BsonDateTime(setTime.AddSeconds(captureSecond)),
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session is never called.");

            var client = CustomHost.Create(sessionMock: sessionMock, dateMock: dateMock);

            var request = CustomRequest.Create(HttpMethod.Get, $"/sessions/EXEX");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var data = BsonSerializer.Deserialize<SessionSchema>(body);

            Assert.True(data.IsOngoing == isOngoing, "isOngoing is not aligned with given value.");
        }

        [Fact]
        public async Task RequiresAccessDeleteSession()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Delete, "/sessions/EXEX", false);
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Getting a session is a bad request without access token.");
        }

        [Fact]
        public async Task SessionIsNotFoundDeleteSession()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Delete, "/sessions/EXEX");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.NotFound,
                "Getting missing session is not found.");
        }

        [Fact]
        public async Task FindAndUpdateAreCalledDeleteSession()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema();
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            sessionMock.Setup(s => s.Update(
                    It.IsAny<FilterDefinition<SessionSchema>>(),
                    It.IsAny<UpdateDefinition<SessionSchema>>()))
                .Verifiable("A session was never updated.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Delete, "/sessions/EXEX");
            var response = await client.SendAsync(request);
        }

        [Theory]
        [InlineData("f")]
        [InlineData("EXEX")]
        [InlineData("EXEX/")]
        [InlineData("EXEX?")]
        [InlineData("t34g534ff43ff44")]
        [InlineData("t34g534ff43ff44?wrd23=23d")]
        public async Task MultipleRoutesAllowedDeleteSession(string input)
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema();
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Delete, $"/sessions/{input}");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ResponceIsOkDeleteSession()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema();
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Delete, $"/sessions/EXEX");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.True(body == "OK", "Delete session responce body is no 'OK'.");
        }

        [Theory]
        [InlineData("t")]
        [InlineData("534r3wefv3c")]
        [InlineData("EXEX")]
        [InlineData("EXEX/")]
        [InlineData("EXEX?")]
        [InlineData("EXEX/?")]
        [InlineData("EXEX/?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task OtherMethodsSession(string input)
        {
            var client = CustomHost.Create();

            var requestPut = CustomRequest.Create(HttpMethod.Put, $"/sessions/{input}");
            var responsePut = await client.SendAsync(requestPut);

            Assert.True(
                responsePut.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Putting a session is an allowed method.");

            var requestPatch = CustomRequest.Create(HttpMethod.Patch, $"/sessions/{input}");
            var responsePatch = await client.SendAsync(requestPatch);

            Assert.True(
                responsePatch.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Patching a session is an allowed method.");
        }
    }
}