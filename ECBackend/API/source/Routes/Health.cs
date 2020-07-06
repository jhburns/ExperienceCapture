namespace Carter.App.Route.Health
{
    using Carter;

    using Carter.App.Lib.Network;

    using Microsoft.Extensions.Logging;

    public class Health : CarterModule
    {
        public Health(ILogger logger)
        {
            this.Get("/", async (req, res) =>
            {
                // Print here for reference, and to make tutorials more clear
                logger.LogInformation("Hello World!");
                await res.FromString("The api server is running.");
            });

            this.Get("/health", async (req, res) =>
            {
                await res.FromString();
            });
        }
    }
}