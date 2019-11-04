namespace Nancy.App.Hosting.Kestrel
{
    public class Health : NancyModule
    {
        public Health(IAppConfiguration appConfig)
        {
            this.Get("/", args => "The receiving server is running.");

            this.Get("/health", args => "OK");
        }
    }
}