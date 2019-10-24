namespace Nancy.App.Hosting.Kestrel
{
    using Nancy;
    using Nancy.TinyIoc;

    using MongoDB.Driver;

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

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            MongoClient client = new MongoClient(@"mongodb://db:27017");
            container.Register<MongoClient>(client);

            container.Register<IAppConfiguration>(appConfig);
        }
    }
}