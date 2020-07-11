namespace Carter.App.Route.Health
{
    using Carter;

    using Carter.App.Lib.Network;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Health and root routes.
    /// </summary>
    public class Health : CarterModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Health"/> class.
        /// </summary>
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