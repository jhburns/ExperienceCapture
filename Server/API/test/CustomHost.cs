namespace Carter.Tests.CustomHost
{
    using System.Net.Http;

    using Carter.App.Hosting;
    using Carter.App.Lib.Environment;
    using Carter.App.Lib.MinioExtra;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using MongoDB.Driver;

    using Moq;

    public static class CustomHost
    {
        public static HttpClient Create()
        {
            var server = new TestServer(
                new WebHostBuilder()
                    .ConfigureServices(services =>
                    {
                        services.AddCarter();

                        var databaseMock = new Mock<IMongoDatabase>();
                        services.AddSingleton<IMongoDatabase>(databaseMock.Object);

                        var objectStoreMock = new Mock<IMinioClient>();
                        services.AddSingleton<IMinioClient>(objectStoreMock.Object);

                        var envMock = new Mock<IAppEnvironment>();
                        services.AddSingleton<IAppEnvironment>(envMock.Object);

                        var logger = Mock.Of<ILogger<Program>>();
                        services.AddSingleton<ILogger>(logger);
                    })
                    .Configure(x =>
                    {
                        x.UseRouting();
                        x.UseEndpoints(builder => builder.MapCarter());
                    }));

            return server.CreateClient();
        }
    }
}
