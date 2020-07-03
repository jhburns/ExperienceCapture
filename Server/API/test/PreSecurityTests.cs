namespace Carter.Tests.Route.PreSecurity
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Carter.App.Lib.Repository;
    using Carter.App.Route.Users;

    using Carter.Tests.HostingExtra;

    using MongoDB.Bson;
    using MongoDB.Driver;

    using Moq;

    using Xunit;

    // These tests use an arbitrary route, to make things easier
    public class PreSecurityTests
    {
        [Fact]
        public async Task NoCookieIsBadPreSecurity()
        {
            var client = CustomHost.Create();

            var stringContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/authentication/signUps/", stringContent);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Triggering pre-security without a token is not bad.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("allow")]
        [InlineData("dsf5ter4fdsfd58eidjfhgtuiejdfssdfdsf")]
        [InlineData("^&*(*&^%$%^&**><KOPL<KJHY")]
        public async Task BadAccessTokenIsUnauthorizedPresecurity(string value)
        {
            var accessMock = new Mock<IRepository<AccessTokenSchema>>();
            var result = new Task<AccessTokenSchema>(() =>
            {
                return null;
            });
            result.Start();

            accessMock.Setup(a => a.FindOne(It.IsAny<FilterDefinition<AccessTokenSchema>>()))
                .Returns(result)
                .Verifiable();

            var client = CustomHost.Create(accessMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/authentication/signUps/", false);
            request.Headers.TryAddWithoutValidation("Cookie", "ExperienceCapture-Access-Token=" + value);

            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.Unauthorized,
                "Triggering pre-security with a bad token is not unauthorized.");
        }

        [Theory]
        [InlineData(259201)] // One over expired time
        [InlineData(1000000)]
        public async Task ExpiredTokenIsUnauthorizedPresecurity(int value)
        {
            var accessMock = new Mock<IRepository<AccessTokenSchema>>();
            var result = new Task<AccessTokenSchema>(() =>
            {
                return new AccessTokenSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Hash = string.Empty,
                    User = ObjectId.GenerateNewId(),
                    CreatedAt = new BsonDateTime(DateTime.UtcNow.AddSeconds(-value)),
                    Role = RoleOptions.Admin,
                };
            });
            result.Start();

            accessMock.Setup(a => a.FindOne(It.IsAny<FilterDefinition<AccessTokenSchema>>()))
                .Returns(result)
                .Verifiable();

            var client = CustomHost.Create(accessMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/authentication/signUps/", false);
            request.Headers.TryAddWithoutValidation("Cookie", "ExperienceCapture-Access-Token=" + "ok");

            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.Unauthorized,
                "Triggering pre-security with an expired token is not unauthorized.");
        }

        [Fact]
        public async Task LowerRoleIsUnauthorizedPresecurity()
        {
            var accessMock = new Mock<IRepository<AccessTokenSchema>>();
            var result = new Task<AccessTokenSchema>(() =>
            {
                return new AccessTokenSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Hash = string.Empty,
                    User = ObjectId.GenerateNewId(),
                    CreatedAt = new BsonDateTime(DateTime.UtcNow),
                    Role = RoleOptions.Normal,
                };
            });
            result.Start();

            accessMock.Setup(a => a.FindOne(It.IsAny<FilterDefinition<AccessTokenSchema>>()))
                .Returns(result)
                .Verifiable();

            var client = CustomHost.Create(accessMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/authentication/signUps/", false);
            request.Headers.TryAddWithoutValidation("Cookie", "ExperienceCapture-Access-Token=" + "ok");

            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.Unauthorized,
                "Triggering pre-security with a lower role is not unauthorized.");
        }

        [Fact]
        public async Task AllowedTokenIsOkPresecurity()
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

            var request = CustomRequest.Create(HttpMethod.Post, "/authentication/signUps/");

            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }
    }
}