namespace Carter.App.Hosting
{
    using System;

    using Carter;

    using Docker.DotNet;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Minio;

    using MongoDB.Driver;

    public class Startup
    {
        private readonly AppConfiguration appconfig;

        public Startup(IConfiguration config)
        {
            this.appconfig = new AppConfiguration();
            config.Bind(this.appconfig);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCarter();

            MongoClient client = new MongoClient($"mongodb://{AppConfiguration.ConnectionString}:27017");
            IMongoDatabase db = client.GetDatabase("ec");
            services.AddSingleton<IMongoDatabase>(db);

            MinioClient os = new MinioClient("os:9000", "minio", "minio123");
            services.AddSingleton<MinioClient>(os);

            DockerClient docker = new DockerClientConfiguration(
                new Uri("unix:///var/run/docker.sock"))
                .CreateClient();
            services.AddSingleton<IDockerClient>(docker);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(builder => builder.MapCarter());
        }
    }
}