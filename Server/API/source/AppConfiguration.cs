namespace Carter.App.Hosting
{
    public class AppConfiguration
    {
        public static ServiceConfiguration Mongo
        {
            get;
            set;
        }

        public static ServiceConfiguration Minio
        {
            get;
            set;
        }
    }

    public class ServiceConfiguration
    {
        public string ConnectionString
        {
            get;
            set;
        }

        public int Port
        {
            get;
            set;
        }
    }
}