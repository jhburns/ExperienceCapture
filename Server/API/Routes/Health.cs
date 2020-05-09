namespace Carter.App.Route.Health
{
    using Carter;

    using Carter.App.Lib.Network;

    public class Health : CarterModule
    {
        public Health()
        {
            this.Get("/", async (req, res) =>
            {
                await res.FromString("The api server is running.");
            });

            this.Get("/health", async (req, res) =>
            {
                await res.FromString();
            });
        }
    }
}