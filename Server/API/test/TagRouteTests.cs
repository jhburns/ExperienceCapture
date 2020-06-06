namespace Carter.Tests.Route.PreSecurity
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Carter.App.Lib.Repository;
    using Carter.App.Route.Sessions;

    using Carter.Tests.HostingExtra;

    using MongoDB.Bson;
    using MongoDB.Driver;

    using Moq;

    using Xunit;

    public class TagTests
    {
        [Fact]
        public async Task RequiresAccessPostTag()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions/EXEX/tags/test", false);
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Tagging is a bad request without token.");
        }

        [Fact]
        public async Task PostMissingSessionIsNotFoundPostTag()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();
            var result = new Task<SessionSchema>(() =>
            {
                return null;
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions/EXEX/tags/test");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.NotFound,
                "Tagging a missing sessions is not 'not found'.");
        }

        [Theory]
        [InlineData("isAllowed")]
        [InlineData("&*(*&^%^&*jJ234")]
        public async Task AnyStringIsAllowedPostTag(string value)
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();
            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Id = string.Empty,
                    User = null,
                    CreatedAt = new BsonDateTime(DateTime.Now),
                    Tags = new List<string>(),
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Post, $"/sessions/EXEX/tags/{value}");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task EmptyTagIsNotFoundPostTags()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();
            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Id = string.Empty,
                    User = null,
                    CreatedAt = new BsonDateTime(DateTime.Now),
                    Tags = new List<string>(),
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for");

            var client = CustomHost.Create(sessionMock: sessionMock);
            var request = CustomRequest.Create(HttpMethod.Post, $"/sessions/EXEX/tags/");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.NotFound,
                "Tagging a an empty is not 'not found'.");
        }

        [Fact]
        public async Task ResponceIsOkPostTags()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();
            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Id = string.Empty,
                    User = null,
                    CreatedAt = new BsonDateTime(DateTime.Now),
                    Tags = new List<string>(),
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for");

            var client = CustomHost.Create(sessionMock: sessionMock);
            var request = CustomRequest.Create(HttpMethod.Post, $"/sessions/EXEX/tags/test");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "text/plain; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            Assert.True(
                await response.Content.ReadAsStringAsync() == "OK",
                "Tagging does not have a body of 'OK'");
        }

        [Theory]
        [InlineData("test/")]
        [InlineData("test")]
        [InlineData("_")]
        [InlineData("sdfsdf4erbtg4e&*sada")]
        [InlineData("sdfsdf4erbtg4e&*sada?test=sdkfjsdlfksdf&blak=sdfsfds")]
        [InlineData("test?")]
        [InlineData("test/?")]
        [InlineData("test?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task MultipulePathsAllowedPostTags(string input)
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();
            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Id = string.Empty,
                    User = null,
                    CreatedAt = new BsonDateTime(DateTime.Now),
                    Tags = new List<string>(),
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for");

            var client = CustomHost.Create(sessionMock: sessionMock);
            var request = CustomRequest.Create(HttpMethod.Post, $"/sessions/EXEX/tags/{input}");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task UpdateIsCalledPostTag()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();
            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Id = string.Empty,
                    User = null,
                    CreatedAt = new BsonDateTime(DateTime.Now),
                    Tags = new List<string>(),
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for");

            // For some reason Verify() doesn't work
            sessionMock.Setup(
                s => s.Update(
                    It.IsAny<FilterDefinition<SessionSchema>>(),
                    It.IsAny<UpdateDefinition<SessionSchema>>()))
                .Verifiable("A session was never updated");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Post, $"/sessions/EXEX/tags/test");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task RequiresAccessDeleteTag()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions/EXEX/tags/test", false);
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Tagging is a bad request without token.");
        }

        [Fact]
        public async Task MissingSessionIsNotFoundDeleteTag()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();
            var result = new Task<SessionSchema>(() =>
            {
                return null;
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Delete, "/sessions/EXEX/tags/test");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.NotFound,
                "Tagging a missing sessions is not 'not found'.");
        }

        [Theory]
        [InlineData("isAllowed")]
        [InlineData("&*(*&^%^&*jJ234")]
        public async Task AnyStringIsAllowedDeleteTag(string value)
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();
            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Id = string.Empty,
                    User = null,
                    CreatedAt = new BsonDateTime(DateTime.Now),
                    Tags = new List<string>(),
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Delete, $"/sessions/EXEX/tags/{value}");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task EmptyTagIsNotFoundDeleteTags()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();
            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Id = string.Empty,
                    User = null,
                    CreatedAt = new BsonDateTime(DateTime.Now),
                    Tags = new List<string>(),
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for");

            var client = CustomHost.Create(sessionMock: sessionMock);
            var request = CustomRequest.Create(HttpMethod.Delete, $"/sessions/EXEX/tags/");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.NotFound,
                "Tagging a an empty is not 'not found'.");
        }

        [Fact]
        public async Task ResponceIsOkDeleteTags()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();
            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Id = string.Empty,
                    User = null,
                    CreatedAt = new BsonDateTime(DateTime.Now),
                    Tags = new List<string>(),
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for");

            var client = CustomHost.Create(sessionMock: sessionMock);
            var request = CustomRequest.Create(HttpMethod.Delete, $"/sessions/EXEX/tags/test");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "text/plain; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            Assert.True(
                await response.Content.ReadAsStringAsync() == "OK",
                "Tagging does not have a body of 'OK'");
        }

        [Fact]
        public async Task UpdateIsCalledDeleteTag()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();
            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Id = string.Empty,
                    User = null,
                    CreatedAt = new BsonDateTime(DateTime.Now),
                    Tags = new List<string>(),
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for");

            // For some reason Verify() doesn't work
            sessionMock.Setup(
                s => s.Update(
                    It.IsAny<FilterDefinition<SessionSchema>>(),
                    It.IsAny<UpdateDefinition<SessionSchema>>()))
                .Verifiable("A session was never updated");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Delete, $"/sessions/EXEX/tags/test");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("test/")]
        [InlineData("test")]
        [InlineData("_")]
        [InlineData("sdfsdf4erbtg4e&*sada")]
        [InlineData("sdfsdf4erbtg4e&*sada?test=sdkfjsdlfksdf&blak=sdfsfds")]
        [InlineData("test?")]
        [InlineData("test/?")]
        [InlineData("test?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task MultipulePathsAllowedDeleteTags(string input)
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();
            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Id = string.Empty,
                    User = null,
                    CreatedAt = new BsonDateTime(DateTime.Now),
                    Tags = new List<string>(),
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for");

            var client = CustomHost.Create(sessionMock: sessionMock);
            var request = CustomRequest.Create(HttpMethod.Delete, $"/sessions/EXEX/tags/{input}");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("test/")]
        [InlineData("test?")]
        [InlineData("test/?")]
        [InlineData("test?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task OtherMethodsTag(string input)
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();
            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Id = string.Empty,
                    User = null,
                    CreatedAt = new BsonDateTime(DateTime.Now),
                    Tags = new List<string>(),
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for");

            var client = CustomHost.Create(sessionMock: sessionMock);
            var requestPut = CustomRequest.Create(HttpMethod.Put, $"/sessions/EXEX/tags/{input}");
            var responsePut = await client.SendAsync(requestPut);

            Assert.True(
                responsePut.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Putting tags is an allowed method");

            var requestPatch = CustomRequest.Create(HttpMethod.Put, $"/sessions/EXEX/tags/{input}");
            var responsePatch = await client.SendAsync(requestPatch);

            Assert.True(
                responsePatch.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Patching tags is an allowed method");

            var requestGet = CustomRequest.Create(HttpMethod.Put, $"/sessions/EXEX/tags/{input}");
            var responseGet = await client.SendAsync(requestGet);

            Assert.True(
                responseGet.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Gettings tags is an allowed method");
        }
    }
}