namespace Carter.Tests.Route.Health
{
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Carter.Tests.CustomHost;

    using Xunit;

    public class HealthTests
    {
        [Theory]
        [InlineData("/health")]
        [InlineData("/health/")]
        [InlineData("/health?")]
        [InlineData("/health/?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task GetHealth(string url)
        {
            // Arrange
            var client = CustomHost.Create();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(
                "text/plain; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            string body = await response.Content.ReadAsStringAsync();
            Assert.True(body == "OK", "Health check is not OK string.");
        }

        [Theory]
        [InlineData("{}")]
        [InlineData("{ \"testing\": \"dsf5ter4fdsfd58eidjfhgtuiejdfssdfdsf\" ")]
        [InlineData("a")]
        public async Task OtherMethodHealth(string json)
        {
            var client = CustomHost.Create();

            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            var responsePost = await client.PostAsync("/health", stringContent);

            Assert.True(responsePost.StatusCode == HttpStatusCode.MethodNotAllowed, "Posting health is not a 405 error.");

            var responsePatch = await client.PatchAsync("/health", stringContent);
            Assert.True(responsePatch.StatusCode == HttpStatusCode.MethodNotAllowed, "Patching health is not a 405 error.");

            var responsePut = await client.PutAsync("/health", stringContent);
            Assert.True(responsePut.StatusCode == HttpStatusCode.MethodNotAllowed, "Putting health is not a 405 error.");

            var responseDelete = await client.DeleteAsync("/health");
            Assert.True(responseDelete.StatusCode == HttpStatusCode.MethodNotAllowed, "Deleting health is not a 405 error.");
        }
    }

    public class RootTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("/")]
        [InlineData("/?")]
        [InlineData("/?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task GetRoot(string url)
        {
            var client = CustomHost.Create();

            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "text/plain; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            string body = await response.Content.ReadAsStringAsync();
            Assert.True(body.Length > 0, "Base responce has no content.");
        }

        [Theory]
        [InlineData("{}")]
        [InlineData("{ \"testing\": \"dsf5ter4fdsfd58eidjfhgtuiejdfssdfdsf\" ")]
        [InlineData("a")]
        public async Task OtherMethodRoot(string json)
        {
            var client = CustomHost.Create();

            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            var responsePost = await client.PostAsync("/", stringContent);

            Assert.True(responsePost.StatusCode == HttpStatusCode.MethodNotAllowed, "Posting root is not a 405 error.");

            var responsePatch = await client.PatchAsync("/", stringContent);
            Assert.True(responsePatch.StatusCode == HttpStatusCode.MethodNotAllowed, "Patching root is not a 405 error.");

            var responsePut = await client.PutAsync("/", stringContent);
            Assert.True(responsePut.StatusCode == HttpStatusCode.MethodNotAllowed, "Putting root is not a 405 error.");

            var responseDelete = await client.DeleteAsync("/");
            Assert.True(responseDelete.StatusCode == HttpStatusCode.MethodNotAllowed, "Deleting root is not a 405 error.");
        }
    }
}