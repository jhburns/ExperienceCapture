namespace Carter.Tests.Route.Health
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Carter.Tests.HostingExtra;

    using Xunit;

    public class IllegalTests
    {
        [Theory]
        [InlineData("/fail/")]
        [InlineData("/sessions/test/tag/notallowed/")]
        [InlineData("/session/test/tag/notallowed")]
        [InlineData("/fail/?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task IllegalUrlIsNotFoundGet(string input)
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Get, input);
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.NotFound,
                $"The following illegal url is allowed: {input}");
        }

        [Theory]
        [InlineData("/fail/")]
        [InlineData("/sessions/test/tag/notallowed/")]
        [InlineData("/session/test/tag/notallowed")]
        [InlineData("/fail/?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task IllegalUrlIsNotFoundPost(string input)
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Post, input);
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.NotFound,
                $"The following illegal url is allowed: {input}");
        }

        [Theory]
        [InlineData("/fail/")]
        [InlineData("/sessions/test/tag/notallowed/")]
        [InlineData("/session/test/tag/notallowed")]
        [InlineData("/fail/?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task IllegalUrlIsNotFoundPatch(string input)
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Patch, input);
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.NotFound,
                $"The following illegal url is allowed: {input}");
        }

        [Theory]
        [InlineData("/fail/")]
        [InlineData("/sessions/test/tag/notallowed/")]
        [InlineData("/session/test/tag/notallowed")]
        [InlineData("/fail/?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task IllegalUrlIsNotFoundPut(string input)
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Put, input);
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.NotFound,
                $"The following illegal url is allowed: {input}");
        }

        [Theory]
        [InlineData("/fail/")]
        [InlineData("/sessions/test/tag/notallowed/")]
        [InlineData("/session/test/tag/notallowed")]
        [InlineData("/fail/?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task IllegalUrlIsNotFoundDelete(string input)
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Delete, input);
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.NotFound,
                $"The following illegal url is allowed: {input}");
        }
    }
}