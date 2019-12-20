/*namespace Nancy.App.Hosting.Kestrel
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Nancy.Owin;

    public class Startup
    {
        private readonly IConfiguration config;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                              .AddJsonFile("appsettings.json")
                              .SetBasePath(env.ContentRootPath);

            this.config = builder.Build();
        }

        public void Configure(IApplicationBuilder app)
        {
            var appConfig = new AppConfiguration();
            ConfigurationBinder.Bind(this.config, appConfig);

            app.UseOwin(x => x.UseNancy(opt => opt.Bootstrapper = new Bootstrapper(appConfig)));
        }
    }
}
*/

namespace Carter.App.Hosting
{
    using Carter;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

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
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(builder => builder.MapCarter());
        }
    }
}