namespace Carter.App.Lib.Authentication
{
    using System;
    using System.Threading.Tasks;

    using Google.Apis.Auth;

    public class GoogleApi
    {
        public static async Task<bool> ValidateUser(string idToken)
        {
            string skipValidation = Environment.GetEnvironmentVariable("unsafe_do_no_validate_user");

            if (skipValidation != "true")
            {
                var validPayload = await GoogleJsonWebSignature.ValidateAsync(idToken);

                if (validPayload == null) 
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return true;
        }
    }
}
