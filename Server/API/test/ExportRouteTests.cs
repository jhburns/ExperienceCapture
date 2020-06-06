namespace Carter.Tests.Route.PreSecurity
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Carter.App.Lib.ExporterExtra;
    using Carter.App.Lib.Repository;
    using Carter.App.Route.Export;

    using Carter.Tests.HostingExtra;

    using MongoDB.Driver;

    using Moq;

    using Xunit;

    public class ExportTests
    {
        [Fact]
        public async Task RequiresAccessPostExport()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions/EXEX/export", false);
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Creating an export is a bad request without access token.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("/")]
        [InlineData("?")]
        [InlineData("/?")]
        [InlineData("/?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task OtherMethodsExport(string input)
        {
            var client = CustomHost.Create();

            var requestPut = CustomRequest.Create(HttpMethod.Put, $"/sessions/EXEX/export{input}");
            var responsePut = await client.SendAsync(requestPut);

            Assert.True(
                responsePut.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Putting export is an allowed method.");

            var requestPatch = CustomRequest.Create(HttpMethod.Patch, $"/sessions/EXEX/export{input}");
            var responsePatch = await client.SendAsync(requestPatch);

            Assert.True(
                responsePatch.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Patching export is an allowed method.");

            var requestDelete = CustomRequest.Create(HttpMethod.Patch, $"/sessions/EXEX/export{input}");
            var responseDelete = await client.SendAsync(requestDelete);

            Assert.True(
                responseDelete.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Deleting export is an allowed method.");
        }
    }
}