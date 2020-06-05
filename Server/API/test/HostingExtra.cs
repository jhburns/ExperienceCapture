namespace Carter.Tests.HostingExtra
{
    using System.Net.Http;
    using System.Text;

    using Carter.App.Hosting;
    using Carter.App.Lib.Environment;
    using Carter.App.Lib.MinioExtra;
    using Carter.App.Lib.Repository;
    using Carter.App.Route.NewSignUp;
    using Carter.App.Route.Users;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using MongoDB.Driver;

    using Moq;

    public static class CustomHost
    {
        public static HttpClient Create(
            Mock<IRepository<AccessTokenSchema>> accessMock = null,
            Mock<IRepository<SignUpTokenSchema>> signUpMock = null,
            Mock<IMongoDatabase> databaseMock = null)
        {
            var server = new TestServer(
                new WebHostBuilder()
                    .ConfigureServices(services =>
                    {
                        services.AddCarter();

                        // Mock repos
                        if (accessMock == null)
                        {
                            accessMock = new Mock<IRepository<AccessTokenSchema>>();
                        }

                        services.AddSingleton<IRepository<AccessTokenSchema>>(accessMock.Object);

                        if (signUpMock == null)
                        {
                            signUpMock = new Mock<IRepository<SignUpTokenSchema>>();
                        }

                        services.AddSingleton<IRepository<SignUpTokenSchema>>(signUpMock.Object);

                        // Mock database
                        if (databaseMock == null)
                        {
                            var collection = new Mock<IMongoCollection<It.IsAnyType>>();
                            collection.SetupAllProperties();

                            databaseMock = new Mock<IMongoDatabase>();
                            databaseMock.Setup(db => db.GetCollection<It.IsAnyType>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                                .Returns(collection.Object);
                        }

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

    public static class CustomRequest
    {
        public static HttpRequestMessage Create(HttpMethod method, string url)
        {
            var request = new HttpRequestMessage(method, "/users/signUp/");
            request.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

            return request;
        }
    }
}