namespace Carter.App.Validation.Person
{
    using FluentValidation;

    /// <summary>
    /// Schema for new user request.
    /// </summary>
    public class PersonRequest
    {
        #pragma warning disable SA1516, SA1300

        /// <summary>Google JWT to be verified.</summary>
        public string idToken { get; set; }

        /// <summary>Base64 encoded.</summary>
        public string signUpToken { get; set; }
        #pragma warning restore SA151, SA1300
    }

    /// <summary>
    /// Validator for new user request.
    /// </summary>
    public class PersonValidator : AbstractValidator<PersonRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersonValidator"/> class.
        /// </summary>
        public PersonValidator()
        {
            this.RuleFor(x => x.idToken).NotEmpty();
            this.RuleFor(x => x.signUpToken).NotEmpty();
        }
    }
}