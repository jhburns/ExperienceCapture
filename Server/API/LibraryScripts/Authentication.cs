namespace Carter.App.Lib.Authentication
{
    using System;

    using Google.Apis;

    public class GoogleApi
    {
        public static bool ValidateUser()
        {
            string skipValidation = Environment.GetEnvironmentVariable("unsafe_do_no_validate_user");

            if (skipValidation != "true")
            {
                return false;
            }

            return true;
        }
    }
}
