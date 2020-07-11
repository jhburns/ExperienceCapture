namespace Carter.App.Hosting
{
    /// <summary>
    /// Schema for app configuration.
    /// </summary>
    public class AppConfiguration
    {
        /// <summary>Configuration for MongoDB.</summary>
        public static ConnectionConfiguration Mongo { get; set; }

        /// <summary>Configuration for Minio.</summary>
        public static ConnectionConfiguration Minio { get; set; }

        /// <summary>Configuration for Carter.</summary>
        public CarterOptions CarterOptions { get; set; }
    }

    /// <summary>
    /// Access to OpenAPI configuration.
    /// </summary>
    public class CarterOptions
    {
        /// <summary>OpenAPI customization.</summary>
        public OpenApi OpenApi { get; set; }
    }

    /// <summary>
    /// OpenAPI customization.
    /// </summary>
    public class OpenApi
    {
        /// <summary>Title of the OpenAPI document.</summary>
        public string DocumentTitle { get; set; }

        /// <summary>Warning: this does not do anything.</summary>
        public string Version { get; set; }
    }

    /// <summary>
    /// General class to connect to databases.
    /// </summary>
    public class ConnectionConfiguration
    {
        /// <summary>The url that will be connected to.</summary>
        public string ConnectionString { get; set; }

        /// <summary>The port that will be used.</summary>
        public int Port { get; set; }
    }
}