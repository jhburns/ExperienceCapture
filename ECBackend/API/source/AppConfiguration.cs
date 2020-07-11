namespace Carter.App.Hosting
{
    /// <summary>
    /// Schema for app configuration.
    /// </summary>
    public class AppConfiguration
    {
        /// <value>Configuration for MongoDB.</value>
        public static ConnectionConfiguration Mongo { get; set; }

        /// <value>Configuration for Minio.</value>
        public static ConnectionConfiguration Minio { get; set; }

        /// <value>Configuration for Carter.</value>
        public CarterOptions CarterOptions { get; set; }
    }

    /// <summary>
    /// Access to OpenAPI configuration.
    /// </summary>
    public class CarterOptions
    {
        /// <value>OpenAPI customization.</value>
        public OpenApi OpenApi { get; set; }
    }

    /// <summary>
    /// OpenAPI customization.
    /// </summary>
    public class OpenApi
    {
        /// <value>Title of the OpenAPI document.</value>
        public string DocumentTitle { get; set; }

        /// <value>Warning: this does not do anything.</value>
        public string Version { get; set; }
    }

    /// <summary>
    /// General class to connect to databases.
    /// </summary>
    public class ConnectionConfiguration
    {
        /// <value>The url that will be connected to.</value>
        public string ConnectionString { get; set; }

        /// <value>The port that will be used.</value>
        public int Port { get; set; }
    }
}