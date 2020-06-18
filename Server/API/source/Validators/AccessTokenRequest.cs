namespace Carter.App.Validation.AccessTokenRequest
{
    using FluentValidation;

    public class AccessTokenRequest
    {
        #pragma warning disable SA1516, SA1300
        public string idToken { get; set; }
        public string claimToken { get; set; }
        #pragma warning restore SA151, SA1300
    }

    public class AccessTokenValidator : AbstractValidator<AccessTokenRequest>
    {
        public AccessTokenValidator()
        {
            this.RuleFor(x => x.idToken).NotEmpty();
        }
    }
}