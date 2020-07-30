namespace Carter.App.Route.Health
{
    using Carter;

    using Carter.App.Libs.Network;
    using Carter.App.MetaData.Health;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Health and root routes.
    /// </summary>
    public class Health : CarterModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Health"/> class.
        /// </summary>
        /// <param name="logger">Supplied through DI.</param>
        public Health(ILogger logger)
        {
            this.Get<GetRoot>("/", async (req, res) =>
            {
                // Print here for reference, and to make tutorials more clear
                logger.LogInformation("Hello World!");
                await res.FromString("The api server is running.");
            });

            this.Get<GetHealth>("/health", async (req, res) =>
            {
                await res.FromString();
            });
        }
    }
}