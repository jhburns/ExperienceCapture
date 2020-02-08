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

        // TODO: CHeck/fix config so OpenAPI is customizable
        public Startup(IConfiguration config)
        {
            this.appconfig = new AppConfiguration();
            config.Bind(this.appconfig);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCarter();

            // TODO: change port number to be included in config
            MongoClient client = new MongoClient($"mongodb://{AppConfiguration.ConnectionString}:27017");
            IMongoDatabase db = client.GetDatabase("ec");
            services.AddSingleton<IMongoDatabase>(db);

            string minioUsername = "minio";
            string minioPassword = "minio123";
            services.AddSingleton<string>(minioUsername);
            services.AddSingleton<string>(minioPassword);

            // TODO: change this to use config string like Mongo
            MinioClient os = new MinioClient("os:9000", minioUsername, minioPassword);
            services.AddSingleton<MinioClient>(os);

            // TODO: Check if this work on Docker for Windows
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