namespace Carter.App.Hosting
{
    using Carter;

    using Carter.App.Lib.Environment;
    using Carter.App.Lib.MinioExtra;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using MongoDB.Driver;

    public class Startup
    {
        private readonly AppConfiguration appConfig;

        // TODO: Check/fix config so OpenAPI is customizable
        public Startup(IConfiguration config)
        {
            this.appConfig = new AppConfiguration();
            config.Bind(this.appConfig);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCarter();

            string mongoUrl = $"mongodb://{AppConfiguration.Mongo.ConnectionString}:{AppConfiguration.Mongo.Port}";
            var client = new MongoClient(mongoUrl);
            var db = client.GetDatabase("ec");
            services.AddSingleton<IMongoDatabase>(db);

            string minioUsername = "minio";
            string minioPassword = "minio123";

            string minioHost = $"{AppConfiguration.Minio.ConnectionString}:{AppConfiguration.Minio.Port}";
            var os = new MinioClientExtra(minioHost, minioUsername, minioPassword);
            services.AddSingleton<IMinioClient>(os);

            var env = ConfigureAppEnvironment.FromEnv();
            services.AddSingleton<IAppEnvironment>(env);

            // TODO: Fix this so that LogDebug() isn't ignored
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<Program>();
            services.AddSingleton<ILogger>(logger);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(builder => builder.MapCarter());
        }
    }
}