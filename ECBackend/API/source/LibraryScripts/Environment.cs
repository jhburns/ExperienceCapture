namespace Carter.App.Lib.Environment
{
    using System;

    /// <summary>
    /// Contains all of the environment variables needs by the application.
    /// </summary>
    public interface IAppEnvironment
    {
        #pragma warning disable SA1516

        /// <summary>
        /// The hash of the admin password, not the password itself.
        /// </summary>
        string PasswordHash { get; }

        /// <summary>
        /// Whether to skip validation, either "yes" for true or anything else for false.
        /// </summary>
        string SkipValidation { get; }

        /// <summary>
        /// The Google client id.
        /// </summary>
        string Audience { get; }

        /// <summary>
        /// The website domain name.
        /// </summary>
        string Domain { get; }
        #pragma warning restore SA151, SA1300
    }

    /// <summary>
    /// Setups up the environment.
    /// </summary>
    public static class ConfigureAppEnvironment
    {

        /// <summary>
        /// Builds the app environment from environmental variables.
        /// </summary>
        /// <returns>
        /// The app environment.
        /// </returns>
        public static AppEnvironment FromEnv()
        {
            string passwordHash = Environment.GetEnvironmentVariable("admin_password_hash")
                    ?? throw new EnvironmentVarNotSet("The following is unset", "admin_password_hash");

            string skipValidation = Environment.GetEnvironmentVariable("unsafe_do_no_validate_user")
                    ?? "false";

            string audience = Environment.GetEnvironmentVariable("gcp_client_id")
                    ?? throw new EnvironmentVarNotSet("The following is unset", "gcp_client_id");

            string domain = Environment.GetEnvironmentVariable("aws_domain_name")
                    ?? throw new EnvironmentVarNotSet("The following is unset", "aws_domain_name");

            return new AppEnvironment(passwordHash, skipValidation, audience, domain);
        }
    }

    /// <summary>
    /// Implements the IAppEnvironment.
    /// </summary>
    public class AppEnvironment : IAppEnvironment
    {
        private string passwordHash;
        private string skipValidation;
        private string audience;
        private string domain;

        /// <summary>
        /// Constructor that requires each of the variable values.
        /// </summary>
        /// <returns>
        /// The app environment.
        /// </returns>
        /// <param name="p">The password hash.</param>
        /// <param name="s">The skip validation value.</param>
        /// <param name="a">The audience.</param>
        /// <param name="d">The domain.</param>
        public AppEnvironment(string p, string s, string a, string d)
        {
            this.passwordHash = p;
            this.skipValidation = s;
            this.audience = a;
            this.domain = d;
        }

        /// <value>Gets the value of passwordHash.</value>
        public string PasswordHash
        {
            get => this.passwordHash;
        }

        /// <value>Gets the value of skipValidation.</value>
        public string SkipValidation
        {
            get => this.skipValidation;
        }

        /// <value>Gets the value of audience.</value>
        public string Audience
        {
            get => this.audience;
        }

        /// <value>Gets the value of domain.</value>
        public string Domain
        {
            get => this.domain;
        }
    }

    /// <summary>
    /// Covers the case when an environment variable is not set.
    /// </summary>
    public class EnvironmentVarNotSet : Exception
    {
        /// <summary>
        /// Constructor with no message.
        /// </summary>
        public EnvironmentVarNotSet()
        {
        }

        /// <summary>
        /// Constructor with message.
        /// </summary>
        /// <param name="message">Information about why this exception is thrown.</param>
        public EnvironmentVarNotSet(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor with message.
        /// </summary>
        /// <param name="message">Information about why this exception is thrown.</param>
        /// <param name="varName">The unset variable.</param>
        public EnvironmentVarNotSet(string message, string varName)
            : base(string.Format("{0}: environment variable {1}", message, varName))
        {
        }

        /// <summary>
        /// Constructor with message.
        /// </summary>
        public EnvironmentVarNotSet(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}