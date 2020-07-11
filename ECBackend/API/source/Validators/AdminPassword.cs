namespace Carter.App.Validation.AdminPassword
{
    using FluentValidation;

    /// <summary>
    /// Schema for admin password request.
    /// </summary>
    public class AdminPasswordRequest
    {
        #pragma warning disable SA1516, SA1300
        /// <value>Base64 encoded.</value>
        public string password { get; set; }
        #pragma warning restore SA151, SA1300
    }

    /// <summary>
    /// Validator for admin password request.
    /// </summary>
    public class AdminPasswordValidator : AbstractValidator<AdminPasswordRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminPasswordValidator"/> class.
        /// </summary>
        public AdminPasswordValidator()
        {
            this.RuleFor(x => x.password).NotEmpty();
        }
    }
}