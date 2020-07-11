namespace Carter.App.Export.Hosting
{
    using Carter.App.Hosting;

    /// <summary>
    /// Schema for exporter configuration.
    /// Passed to the exporter thread on start.
    /// </summary>
    public class ExporterConfiguration
    {
        /// <value>Configuration for MongoDB.</value>
        public ConnectionConfiguration Mongo
        {
            get;
            set;
        }

        /// <value>Configuration for Minio.</value>
        public ConnectionConfiguration Minio
        {
            get;
            set;
        }

        /// <value>A session id passed to the exporter.</value>
        public string Id
        {
            get;
            set;
        }
    }
}