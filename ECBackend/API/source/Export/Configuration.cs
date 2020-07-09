namespace Carter.App.Export.Hosting
{
    using Carter.App.Hosting;

    public class ExporterConfiguration
    {
        public ConnectionConfiguration Mongo
        {
            get;
            set;
        }

        public ConnectionConfiguration Minio
        {
            get;
            set;
        }

        public string Id
        {
            get;
            set;
        }
    }
}