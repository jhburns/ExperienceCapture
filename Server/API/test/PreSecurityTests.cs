namespace Carter.Tests.Route.PreSecurity
{
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Carter.App.Lib.Repository;
    using Carter.App.Route.Users;

    using Carter.Tests.HostingExtra;

    using Moq;

    using Xunit;

    // These tests use an arbitrary route, to make things easier
    public class PreSecurityTests
    {
        [Fact]
        public async Task NoCookieIsBadPreSecurity()
        {
            var accessMock = new Mock<IRepository<AccessTokenSchema>>();
            var client = CustomHost.Create(accessMock);

            var stringContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/users/signUp/", stringContent);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Triggering pre-security with a cookie is not bad.");
        }
    }
}