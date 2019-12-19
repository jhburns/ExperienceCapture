/*namespace Nancy.App.Hosting.Kestrel
{
    using MongoDB.Driver;

    using Nancy;
    using Nancy.Configuration;
    using Nancy.TinyIoc;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private readonly IAppConfiguration appConfig;

        public Bootstrapper()
        {
        }

        public Bootstrapper(IAppConfiguration appConfig)
        {
            this.appConfig = appConfig;
        }

        public override void Configure(INancyEnvironment environment)
        {
            environment.Tracing(enabled: true, displayErrorTraces: true);
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            MongoClient client = new MongoClient(@"mongodb://db:27017");
            IMongoDatabase db = client.GetDatabase("ec");

            container.Register<MongoClient>(client);
            container.Register<IMongoDatabase>(db);

            container.Register<IAppConfiguration>(this.appConfig);
        }
    }
}
*/