namespace Carter.Tests.Route.PreSecurity
{
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using Carter.App.Hosting;
    using Carter.App.Lib.ExporterExtra;
    using Carter.App.Lib.Repository;
    using Carter.App.Route.Export;
    using Carter.App.Route.Sessions;

    using Carter.Tests.HostingExtra;

    using MongoDB.Bson;
    using MongoDB.Driver;

    using Moq;

    using Xunit;

    public class ExportTests
    {
        public ExportTests()
        {
            AppConfiguration.Mongo = new ServiceConfiguration
            {
                ConnectionString = null,
                Port = 0,
            };

            AppConfiguration.Minio = new ServiceConfiguration
            {
                ConnectionString = null,
                Port = 0,
            };
        }

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

        [Fact]
        public async Task IsNotFoundPostExport()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions/EXEX/export");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.NotFound,
                "Exporting a missing session is not 'not found'.");
        }

        [Fact]
        public async Task IsOkIfFoundPostExport()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = null,
                    Id = null,
                    User = null,
                    CreatedAt = null,
                    Tags = null,
                };
            });
            result.Start();

            sessionMock.Setup(a => a.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions/EXEX/export");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.True(
                body == "OK",
                "Exporting responce is not 'OK'.");
        }

        [Fact]
        public async Task UpdateIsCalledPostExport()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = null,
                    Id = null,
                    User = null,
                    CreatedAt = null,
                    Tags = null,
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            sessionMock.Setup(s => s.Update(
                    It.IsAny<FilterDefinition<SessionSchema>>(),
                    It.IsAny<UpdateDefinition<SessionSchema>>()))
                .Verifiable("A session was never updated for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions/EXEX/export");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task RunIsCalledPostExport()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = null,
                    Id = null,
                    User = null,
                    CreatedAt = null,
                    Tags = null,
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var threadMock = new Mock<IThreadExtra>();
            threadMock.Setup(t => t.Run(
                    It.IsAny<ParameterizedThreadStart>(),
                    It.IsAny<object>()))
                .Verifiable("A new exporter thread was never created.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions/EXEX/export");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("")]
        [InlineData("?")]
        [InlineData("/")]
        [InlineData("/?=test=sdkfjsdlfksdf&blak=sdfsfds.")]
        public async Task MultiplePathsAcceptedPostExport(string input)
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = null,
                    Id = null,
                    User = null,
                    CreatedAt = null,
                    Tags = null,
                };
            });
            result.Start();

            sessionMock.Setup(a => a.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Post, $"/sessions/EXEX/export{input}");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
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