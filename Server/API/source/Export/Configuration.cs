namespace Carter.App.Hosting
{
    public class ExporterConfiguration
    {
        public ServiceConfiguration Mongo
        {
            get;
            set;
        }

        public ServiceConfiguration Minio
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