namespace Carter.App.Lib.Authentication
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;

    using System.Threading.Tasks;

    using Carter.App.Lib.Generate;

    using Google.Apis.Auth;

    public class GoogleApi
    {
        // TODO: make following default to empty string
        private static readonly string SkipValidation = Environment.GetEnvironmentVariable("unsafe_do_no_validate_user");

        // TODO: make app throw exception when following not defined
        private static readonly string Audience = Environment.GetEnvironmentVariable("gcp_client_id");

        public static async Task<GoogleJsonWebSignature.Payload> ValidateUser(string idToken)
        {
            if (SkipValidation == "true")
            {
                return new GoogleJsonWebSignature.Payload()
                {
                     Name = "Smitty Jensens",
                     GivenName = "Smitty",
                     FamilyName = "Jensens",
                     Email = "smitty@jenkins.com",
                     Subject = "123456789109876543210", // Subject is longer than Int64
                };
            }

            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new string[] { Audience },
                };

                var validPayload = await GoogleJsonWebSignature.ValidateAsync(idToken);
                Console.WriteLine("Subject: " + validPayload.Subject);
                Console.WriteLine("Audience: " + validPayload.Audience);
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
        // TODO: make app throw exception when following not defined
        private static readonly string Domain = Environment.GetEnvironmentVariable("aws_domain_name");

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
            string urlPassword = WebUtility.UrlEncode(password);
            string hash = Hash(password);
            Console.WriteLine($"Password: {password}");
            Console.WriteLine($"Example login URL for localhost: http://localhost:8090/admin?password={urlPassword}");
            Console.WriteLine($"Example login URL for domain: https://{Domain}/admin?password={urlPassword}");
            Console.WriteLine($"Hash (for .env file): {hash}");
        }
    }
}
