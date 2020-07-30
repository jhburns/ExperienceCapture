namespace Carter.App.Hosting
{
    using System.Collections.Generic;
    using System.IO;

    using Carter;

    using Carter.App.Libs.Environment;
    using Carter.App.Libs.ExporterExtra;
    using Carter.App.Libs.FileExtra;
    using Carter.App.Libs.MinioExtra;
    using Carter.App.Libs.Repository;
    using Carter.App.Libs.Timer;

    using Carter.App.Route.Export;
    using Carter.App.Route.ProtectedUsersAndAuthentication;
    using Carter.App.Route.Sessions;
    using Carter.App.Route.UsersAndAuthentication;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Serializers;
    using MongoDB.Driver;

    #pragma warning disable CS1591
    public class Startup
    {
        private readonly AppConfiguration appConfig;

        public Startup(IConfiguration config)
        {
            this.appConfig = new AppConfiguration();
            config.Bind(this.appConfig);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add and configure Carter
            services.AddCarter((options) =>
            {
                options.OpenApi.DocumentTitle = this.appConfig.CarterOptions.OpenApi.DocumentTitle;
                options.OpenApi.ServerUrls = new[]
                {
                    "http://localhost:8090/api/v1",
                    "https://expcap.xyz/api/v1",
                    "https://expcap2.xyz/api/v1",
                };

                options.OpenApi.Securities = new Dictionary<string, OpenApiSecurity>
                {
                    {
                        "TokenAuthorization",
                        new OpenApiSecurity
                        {
                            Type = OpenApiSecurityType.apiKey,
                            Name = "ExperienceCapture-Access-Token",
                            In = OpenApiIn.cookie,
                        }
                    },
                };
            });

            // Add custom serialization logic for Enums
            BsonSerializer.RegisterSerializer(new EnumSerializer<ExportOptions>(BsonType.String));
            BsonSerializer.RegisterSerializer(new EnumSerializer<RoleOptions>(BsonType.String));

            // Add repositories based on Mongo
            string mongoUrl = $"mongodb://{AppConfiguration.Mongo.ConnectionString}:{AppConfiguration.Mongo.Port}";
            var client = new MongoClient(mongoUrl);
            var db = client.GetDatabase("ec");

            var signUpRepo = new SignUpTokenRepository(db);
            services.AddSingleton<IRepository<SignUpTokenSchema>>(signUpRepo);

            var accessRepo = new AccessTokenRepository(db);
            services.AddSingleton<IRepository<AccessTokenSchema>>(accessRepo);

            var claimRepo = new ClaimTokenRepository(db);
            services.AddSingleton<IRepository<ClaimTokenSchema>>(claimRepo);

            var sessionRepo = new SessionRepository(db);
            services.AddSingleton<IRepository<SessionSchema>>(sessionRepo);

            var captureRepo = new CapturesRepository(db);
            services.AddSingleton<IRepository<BsonDocument>>(captureRepo);

            var personRepo = new PersonRepository(db);
            services.AddSingleton<IRepository<PersonSchema>>(personRepo);

            // Add Threading
            var threader = new ExportThreader();
            services.AddSingleton<IThreadExtra>(threader);

            // Add time
            var date = new DateProvider();
            services.AddSingleton<IDateExtra>(date);

            // Add Minio
            string minioUsername = "minio";
            string minioPassword = "minio123";

            string minioHost = $"{AppConfiguration.Minio.ConnectionString}:{AppConfiguration.Minio.Port}";
            var os = new MinioClientExtra(minioHost, minioUsername, minioPassword);
            services.AddSingleton<IMinioClient>(os);

            // Add environment
            var env = ConfigureAppEnvironment.FromEnv();
            services.AddSingleton<IAppEnvironment>(env);

            // Add logging
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<Program>();
            services.AddSingleton<ILogger>(logger);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseSwaggerUI((options) =>
            {
                options.RoutePrefix = "openapi/ui";

                // Because the site will try to get the page from root,
                // We need to direct it to /api/v1
                options.SwaggerEndpoint("/api/v1/openapi", this.appConfig.CarterOptions.OpenApi.DocumentTitle);

                options.DocumentTitle = "Experience Capture API";

                string seperator = Path.DirectorySeparatorChar.ToString();
                options.HeadContent = FileExtra.GetEmbeddedFile($"Templates{seperator}OpenApiHeader.html");
            });

            app.UseEndpoints(builder => builder.MapCarter());
        }
    }
    #pragma warning restore CS1591
}