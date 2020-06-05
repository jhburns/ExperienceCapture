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
        public async Task RequiresAccessTag()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions/EXEX/tags/test", false);
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Tagging is a bad request without token.");
        }

        [Fact]
        public async Task MissingSessionIsNotFoundTag()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();
            var result = new Task<SessionSchema>(() =>
            {
                return null;
            });
            result.Start();

            sessionMock.Setup(s => s.FindOne(It.IsAny<FilterDefinition<SessionSchema>>()))
                .Returns(result)
                .Verifiable();

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
        public async Task AnyStringIsAllowedTag(string value)
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

            sessionMock.Setup(s => s.FindOne(It.IsAny<FilterDefinition<SessionSchema>>()))
                .Returns(result)
                .Verifiable();

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Post, $"/sessions/EXEX/tags/{value}");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task EmptyTagIsNotFoundTags()
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

            sessionMock.Setup(s => s.FindOne(It.IsAny<FilterDefinition<SessionSchema>>()))
                .Returns(result)
                .Verifiable();

            var client = CustomHost.Create(sessionMock: sessionMock);
            var request = CustomRequest.Create(HttpMethod.Post, $"/sessions/EXEX/tags/");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.NotFound,
                "Tagging a an empty is not 'not found'.");
        }

        [Fact]
        public async Task ResponceIsOkTags()
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

            sessionMock.Setup(s => s.FindOne(It.IsAny<FilterDefinition<SessionSchema>>()))
                .Returns(result)
                .Verifiable();

            var client = CustomHost.Create(sessionMock: sessionMock);
            var request = CustomRequest.Create(HttpMethod.Post, $"/sessions/EXEX/tags/test");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            Assert.True(
                await response.Content.ReadAsStringAsync() == "OK",
                "Tagging does not have a body of 'OK'");
        }
    }
}