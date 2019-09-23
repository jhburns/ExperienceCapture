namespace Nancy.App.Hosting.Kestrel
{
    using ModelBinding;
    using System;

    public class HomeModule : NancyModule
    {
        public HomeModule(IAppConfiguration appConfig)
        {
            Get("/", args => "OK");

            Post("/", args =>
            {
                var person = this.BindAndValidate<Person>();

                if (!this.ModelValidationResult.IsValid)
                {
                    return 422;
                }

                return person;
            });
        }
    }
}