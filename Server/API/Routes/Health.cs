namespace Carter.App.Route.Health
{
    using Carter;
    using Microsoft.AspNetCore.Http;

    public class Health : CarterModule
    {
        public Health()
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