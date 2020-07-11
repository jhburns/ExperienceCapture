namespace Carter.App.Export.Hosting
{
    using Carter.App.Hosting;

    /// <summary>
    /// Schema for exporter configuration.
    /// Passed to the exporter thread on start.
    /// </summary>
    public class ExporterConfiguration
    {
        /// <summary>Configuration for MongoDB.</summary>
        public ConnectionConfiguration Mongo { get; set; }

        /// <summary>Configuration for Minio.</summary>
        public ConnectionConfiguration Minio { get; set; }

        /// <summary>A session id passed to the exporter.</summary>
        public string Id { get; set; }
    }
}