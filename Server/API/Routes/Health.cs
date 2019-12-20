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
            this.Get("/", async (req, res) =>
            {
                await res.WriteAsync("The api server is running.");
            });

            this.Get("/health", async (req, res) =>
            {
                await res.WriteAsync("OK");
            });
        }
    }
}