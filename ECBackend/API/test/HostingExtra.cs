namespace Carter.Tests.HostingExtra
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Carter.App.Hosting;
    using Carter.App.Lib.Environment;
    using Carter.App.Lib.ExporterExtra;
    using Carter.App.Lib.MinioExtra;
    using Carter.App.Lib.Repository;
    using Carter.App.Lib.Timer;

    using Carter.App.Route.ProtectedUsersAndAuthentication;
    using Carter.App.Route.Sessions;
    using Carter.App.Route.UsersAndAuthentication;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using MongoDB.Bson;
    using MongoDB.Driver;

    using Moq;

    public static class CustomHost
    {
        public static HttpClient Create(
            Mock<IRepository<AccessTokenSchema>> accessMock = null,
            Mock<IRepository<SignUpTokenSchema>> signUpMock = null,
            Mock<IRepository<ClaimTokenSchema>> claimMock = null,
            Mock<IRepository<PersonSchema>> personMock = null,
            Mock<IRepository<SessionSchema>> sessionMock = null,
            Mock<IRepository<BsonDocument>> captureMock = null,
            Mock<IMongoDatabase> databaseMock = null,
            Mock<IThreadExtra> threadMock = null,
            Mock<IMinioClient> objectStoreMock = null,
            Mock<IDateExtra> dateMock = null,
            Mock<IAppEnvironment> envMock = null)
        {
            var server = new TestServer(
                new WebHostBuilder()
                    .ConfigureServices(services =>
                    {
                        services.AddCarter();

                        // Mock repos
                        if (accessMock == null)
                        {
                            // Allows every route to authenticate through Pre-security
                            accessMock = new Mock<IRepository<AccessTokenSchema>>();
                            var result = new Task<AccessTokenSchema>(() =>
                            {
                                return new AccessTokenSchema
                                {
                                    // A day so the token can't expire while running
                                    CreatedAt = new BsonDateTime(DateTime.UtcNow.AddSeconds(86400)),
                                    Role = RoleOptions.Normal,
                                };
                            });
                            result.Start();

                            accessMock.Setup(a => a.FindOne(It.IsAny<FilterDefinition<AccessTokenSchema>>()))
                                .Returns(result);
                        }

                        services.AddSingleton<IRepository<AccessTokenSchema>>(accessMock.Object);

                        if (signUpMock == null)
                        {
                            signUpMock = new Mock<IRepository<SignUpTokenSchema>>();
                        }

                        services.AddSingleton<IRepository<SignUpTokenSchema>>(signUpMock.Object);

                        if (claimMock == null)
                        {
                            claimMock = new Mock<IRepository<ClaimTokenSchema>>();
                        }

                        services.AddSingleton<IRepository<ClaimTokenSchema>>(claimMock.Object);

                        if (personMock == null)
                        {
                            personMock = new Mock<IRepository<PersonSchema>>();
                        }

                        services.AddSingleton<IRepository<PersonSchema>>(personMock.Object);

                        if (sessionMock == null)
                        {
                            sessionMock = new Mock<IRepository<SessionSchema>>();
                        }

                        services.AddSingleton<IRepository<SessionSchema>>(sessionMock.Object);

                        if (captureMock == null)
                        {
                            captureMock = new Mock<IRepository<BsonDocument>>();
                        }

                        services.AddSingleton<IRepository<BsonDocument>>(captureMock.Object);

                        if (threadMock == null)
                        {
                            threadMock = new Mock<IThreadExtra>();
                        }

                        services.AddSingleton<IThreadExtra>(threadMock.Object);

                        // Mock object store
                        if (objectStoreMock == null)
                        {
                            objectStoreMock = new Mock<IMinioClient>();
                        }

                        services.AddSingleton<IMinioClient>(objectStoreMock.Object);

                        // Mock timer
                        if (dateMock == null)
                        {
                            services.AddSingleton<IDateExtra>(new DateProvider());
                        }
                        else
                        {
                            services.AddSingleton<IDateExtra>(dateMock.Object);
                        }

                        // Mock environment
                        if (envMock == null)
                        {
                            envMock = new Mock<IAppEnvironment>();
                            envMock.SetupGet(e => e.SkipValidation)
                                .Returns("true");
                        }

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
        public static HttpRequestMessage Create(HttpMethod method, string url, bool isAuthorized = true)
        {
            var request = new HttpRequestMessage(method, url);
            request.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

            if (isAuthorized)
            {
                request.Headers.TryAddWithoutValidation("Cookie", "ExperienceCapture-Access-Token=" + "ok");
            }

            return request;
        }
    }
}