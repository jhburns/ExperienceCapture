namespace Carter.Tests.CustomHost
{
    using System.Net.Http;
    using System.Text;

    using Carter.App.Hosting;
    using Carter.App.Lib.Environment;
    using Carter.App.Lib.MinioExtra;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using MongoDB;
    using MongoDB.Bson;
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

                        // Mock database
                        var collection = new Mock<IMongoCollection<It.IsAnyType>>();
                        collection.SetupAllProperties();

                        var databaseMock = new Mock<IMongoDatabase>();
                        databaseMock.Setup(db => db.GetCollection<It.IsAnyType>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                            .Returns(collection.Object);

                        services.AddSingleton<IMongoDatabase>(databaseMock.Object);

                        // Mock object store
                        var objectStoreMock = new Mock<IMinioClient>();
                        objectStoreMock.SetupAllProperties();
                        services.AddSingleton<IMinioClient>(objectStoreMock.Object);

                        // Mock environment
                        var envMock = new Mock<IAppEnvironment>();
                        envMock.SetupAllProperties();
                        services.AddSingleton<IAppEnvironment>(envMock.Object);

                        // Mock logger
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
