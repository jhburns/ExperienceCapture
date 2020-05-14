namespace Carter.App.Hosting
{
    using Carter;

    using Carter.App.Lib.Environment;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Minio;

    using MongoDB.Driver;

    public class Startup
    {
        private readonly AppConfiguration appconfig;

        // TODO: Check/fix config so OpenAPI is customizable
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

            AppEnvironment env = ConfigureAppEnvironment.FromEnv();
            services.AddSingleton<IAppEnvironment>(env);

            // TODO: Fix this so that LogDebug() isn't ignored
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger logger = loggerFactory.CreateLogger<Program>();
            services.AddSingleton<ILogger>(logger);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(builder => builder.MapCarter());
        }
    }
}