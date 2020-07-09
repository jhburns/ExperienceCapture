namespace Carter.App.Hosting
{
    public class AppConfiguration
    {
        public static ConnectionConfiguration Mongo { get; set; }

        public static ConnectionConfiguration Minio { get; set; }

        public CarterOptions CarterOptions { get; set; }
    }

    public class CarterOptions
    {
        public OpenApi OpenApi { get; set; }
    }

    public class OpenApi
    {
        public string DocumentTitle { get; set; }

        public string Version { get; set; }
    }

    public class ConnectionConfiguration
    {
        public string ConnectionString { get; set; }

        public int Port { get; set; }
    }
}