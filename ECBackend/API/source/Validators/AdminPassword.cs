namespace Carter.App.Validation.AdminPassword
{
    using FluentValidation;

    public class AdminPasswordRequest
    {
        #pragma warning disable SA1516, SA1300
        public string password { get; set; }
        #pragma warning restore SA151, SA1300
    }

    public class AdminPasswordValidator : AbstractValidator<AdminPasswordRequest>
    {
        public AdminPasswordValidator()
        {
            this.RuleFor(x => x.password).NotEmpty();
        }
    }
}