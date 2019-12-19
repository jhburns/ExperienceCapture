/*namespace Nancy.App.Hosting.Kestrel
{
    public class Health : NancyModule
    {
        public Health(IAppConfiguration appConfig)
        {
            this.Get("/", args => "The api server is running.");

            this.Get("/health", args => "OK");
        }
    }
}
*/

namespace Carter.Route.Health
{
    using Carter;
    using Microsoft.AspNetCore.Http;

    public class HomeModule : CarterModule
    {
        public HomeModule()
        {
            this.Get("/", (req, res) =>
            {
                return res.WriteAsync("The api server is running.");
            });

            this.Get("/health", (req, res) =>
            {
                return res.WriteAsync("OK");
            });
        }
    }
}