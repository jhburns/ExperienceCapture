namespace Nancy.App.Hosting.Kestrel
{
    public class Health : NancyModule
    {
        public Health(IAppConfiguration appConfig)
        {
            Get("/", args => "The receiving server is running.");

            Get("/health", args => "OK");
        }
    }
}