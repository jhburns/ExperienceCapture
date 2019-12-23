namespace Carter.App.Lib.Authentication
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;

    using System.Threading.Tasks;

    using Carter.App.Lib.Generate;

    using Google.Apis.Auth;

    public class GoogleApi
    {
        private static readonly string SkipValidation = Environment.GetEnvironmentVariable("unsafe_do_no_validate_user");
        private static readonly string Audience = Environment.GetEnvironmentVariable("aws_domain_name");

        public static async Task<GoogleJsonWebSignature.Payload> ValidateUser(string idToken)
        {
            if (SkipValidation == "true")
            {
                return new GoogleJsonWebSignature.Payload()
                {
                     Name = "Smitty Jenkins",
                     GivenName = "Smitty",
                     FamilyName = "Jenkins",
                     Email = "smitty@jenkins.com",
                };
            }

            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new string[] { "https://" + Audience }, // Token should only be sent over https anyway
                };

                var validPayload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                return validPayload;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }

    public class PasswordHasher
    {
        public static string Hash(string password)
        {
            try
            {
                string hash;
                using (SHA256 sha = new SHA256Managed())
                {
                    byte[] passwordDecoded = Convert.FromBase64String(password);
                    hash = Convert.ToBase64String(sha.ComputeHash(passwordDecoded));
                }

                return hash;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static bool Check(string password, string hash)
        {
            try
            {
                byte[] newHash;
                using (SHA256 sha = new SHA256Managed())
                {
                    byte[] passwordDecoded = Convert.FromBase64String(password);
                    newHash = sha.ComputeHash(passwordDecoded);
                }

                Console.WriteLine(hash);
                Console.WriteLine(Convert.ToBase64String(newHash));
                return Convert.FromBase64String(hash).SequenceEqual(newHash);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public static void OutputNew()
        {
            string password = Generate.GetRandomToken();
            string hash = Hash(password);
            Console.WriteLine($"Password: {password}");
            Console.WriteLine($"Hash: {hash}");
        }
    }
}
