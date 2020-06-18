namespace Carter.App.Validation.Person
{
    using FluentValidation;

    public class PersonRequest
    {
        #pragma warning disable SA1516, SA1300
        public string idToken { get; set; }
        public string signUpToken { get; set; }
        #pragma warning restore SA151, SA1300
    }

    public class PersonValidator : AbstractValidator<PersonRequest>
    {
        public PersonValidator()
        {
            this.RuleFor(x => x.idToken).NotEmpty();
            this.RuleFor(x => x.signUpToken).NotEmpty();
        }
    }
}