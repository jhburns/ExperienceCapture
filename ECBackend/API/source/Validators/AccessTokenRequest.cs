namespace Carter.App.Validation.AccessTokenRequest
{
    using FluentValidation;

    /// <summary>
    /// Schema for access token request.
    /// </summary>
    public class AccessTokenRequest
    {
        #pragma warning disable SA1516, SA1300

        /// <value>Google JWT to be verified.</value>
        public string idToken { get; set; }

        /// <value>Base64 encoded. Optional.</value>
        public string claimToken { get; set; }
        #pragma warning restore SA151, SA1300
    }

    /// <summary>
    /// Validator for access token request.
    /// </summary>
    public class AccessTokenValidator : AbstractValidator<AccessTokenRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccessTokenValidator"/> class.
        /// </summary>
        public AccessTokenValidator()
        {
            this.RuleFor(x => x.idToken).NotEmpty();
        }
    }
}