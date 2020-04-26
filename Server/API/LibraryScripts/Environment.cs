namespace Carter.App.Lib.Environment
{
    using System;

    public interface IAppEnvironment
    {
        #pragma warning disable SA1516
        string PasswordHash { get; }
        string SkipValidation { get; }
        string Audience { get; }
        string Domain { get; }
        #pragma warning restore SA151, SA1300
    }

    public static class ConfigureAppEnvironment
    {
        public static AppEnvironment FromEnv()
        {
            string passwordHash = Environment.GetEnvironmentVariable("admin_password_hash")
                    ?? throw new EnviromentVarNotSet("The following is unset", "admin_password_hash");

            string skipValidation = Environment.GetEnvironmentVariable("unsafe_do_no_validate_user")
                    ?? throw new EnviromentVarNotSet("The following is unset", "unsafe_do_no_validate_user");

            string audience = Environment.GetEnvironmentVariable("gcp_client_id")
                    ?? throw new EnviromentVarNotSet("The following is unset", "gcp_client_id");

            string domain = Environment.GetEnvironmentVariable("aws_domain_name")
                    ?? throw new EnviromentVarNotSet("The following is unset", "aws_domain_name");

            return new AppEnvironment(passwordHash, skipValidation, audience, domain);
        }
    }

    public class AppEnvironment : IAppEnvironment
    {
        private string passwordHash;
        private string skipValidation;
        private string audience;
        private string domain;

        public AppEnvironment(string p, string s, string a, string d)
        {
            this.passwordHash = p;
            this.skipValidation = s;
            this.audience = a;
            this.domain = d;
        }

        public string PasswordHash
        {
            get => this.passwordHash;
        }

        public string SkipValidation
        {
            get => this.skipValidation;
        }

        public string Audience
        {
            get => this.audience;
        }

        public string Domain
        {
            get => this.domain;
        }
    }

    public class EnviromentVarNotSet : Exception
    {
        public EnviromentVarNotSet()
        {
        }

        public EnviromentVarNotSet(string message)
            : base(message)
        {
        }

        public EnviromentVarNotSet(string message, string varName)
            : base(string.Format("{0}: environment variable {1}", message, varName))
        {
        }

        public EnviromentVarNotSet(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}