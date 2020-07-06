namespace Carter.App.Hosting
{
    using Carter;

    using Carter.App.Lib.Environment;
    using Carter.App.Lib.ExporterExtra;
    using Carter.App.Lib.MinioExtra;
    using Carter.App.Lib.Repository;
    using Carter.App.Lib.Timer;

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

            // Add repositories based on Mongo
            BsonSerializer.RegisterSerializer(new EnumSerializer<ExportOptions>(BsonType.String));
            BsonSerializer.RegisterSerializer(new EnumSerializer<RoleOptions>(BsonType.String));

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

            // Note: logger.LogInfo does not work
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