namespace Carter.App.Lib.Authentication
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;

    using System.Threading.Tasks;

    using Carter.App.Lib.Environment;
    using Carter.App.Lib.Generate;

    using Google.Apis.Auth;

    public class GoogleApi
    {
        public static async Task<GoogleJsonWebSignature.Payload> ValidateUser(string idToken, IAppEnvironment env)
        {
            if (env.SkipValidation == "true")
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
                    Audience = new string[] { env.Audience },
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

        public static void OutputNew(string domain)
        {
            string password = Generate.GetRandomToken();
            string urlPassword = WebUtility.UrlEncode(password);
            string hash = Hash(password);
            Console.WriteLine($"Password: {password}");
            Console.WriteLine($"Example login URL for localhost: http://localhost:8090/admin?password={urlPassword}");
            Console.WriteLine($"Example login URL for domain: https://{domain}/admin?password={urlPassword}");
            Console.WriteLine($"Hash (for .env file): {hash}");
        }
    }
}
