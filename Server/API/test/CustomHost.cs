namespace Carter.Tests.CustomHost
{
    using System.Net.Http;

    using Carter.App.Hosting;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Moq;

    public static class CustomHost
    {
        // Generation method only accepts one module
        // In order to promote module seperation
        public static HttpClient Create<TModule>()
            where TModule : CarterModule
        {
            var server = new TestServer(
                new WebHostBuilder()
                    .ConfigureServices(services =>
                    {
                        services.AddCarter(configurator: c =>
                            c.WithModule<TModule>());

                        ILogger logger = Mock.Of<ILogger<Program>>();
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
