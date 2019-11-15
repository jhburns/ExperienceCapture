/*
- Name: Jonathan Hirokazu Burns
- ID: 2288851
- email: jburns@chapman.edu
- Course: 353-01
- Assignment: Submission #2
- Purpose: Starts the server and loads resources
*/

namespace Nancy.App.Hosting.Kestrel
{
    using MongoDB.Driver;

    using Nancy;
    using Nancy.TinyIoc;

    /*
     * Bootstrapper
     * Starts server
     */
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private readonly IAppConfiguration appConfig;

        /*
         * Bootstrapper
         * Default Constructor
         */
        public Bootstrapper()
        {
        }

        /*
         * Bootstrapper
         * Constructor with configuration
         */
        public Bootstrapper(IAppConfiguration appConfig)
        {
            this.appConfig = appConfig;
        }

        /*
         * ConfigureApplicationContainer
         * Params:
         * - container: resource container
         */
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