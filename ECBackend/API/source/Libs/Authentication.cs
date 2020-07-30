namespace Carter.App.Libs.Authentication
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;

    using System.Threading.Tasks;

    using Carter.App.Libs.Environment;
    using Carter.App.Libs.Generate;

    using Google.Apis.Auth;

    /// <summary>
    /// Performs authentication through Google.
    /// </summary>
    public class GoogleApi
    {
        /// <summary>
        /// Sends the jwt token to Google's Servers to be validated.
        /// </summary>
        /// <returns>
        /// The payload received from Google, or null if the token is invalid.
        /// </returns>
        /// <param name="token">A JWT.</param>
        /// <param name="env">The environment, used to check if validation should be skipped.</param>
        public static async Task<GoogleJsonWebSignature.Payload> ValidateUser(string token, IAppEnvironment env)
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

                var validPayload = await GoogleJsonWebSignature.ValidateAsync(token);
                return validPayload;
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Hashes tokens for security purposes.
    /// Keep in mind this only hashes once,
    /// so using this with low entropy passwords is insecure.
    /// </summary>
    public class PasswordHasher
    {
        /// <summary>
        /// Takes a password and computes its SHA256 hash.
        /// </summary>
        /// <returns>
        /// A hash, or null if the given password is not a base64 string.
        /// </returns>
        /// <param name="password">A highly random string, encoded in base64.</param>
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
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Checks whether a password is hashed to a value.
        /// </summary>
        /// <returns>
        /// True, when the password is hashed to the value.
        /// </returns>
        /// <param name="password">A highly random string, encoded in base64.</param>
        /// <param name="hash">A hash value, encoded in base64.</param>
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
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Prints out a new password pretty.
        /// </summary>
        /// <param name="domain">A domain to generate the admin sign up link. Example: localhost:8090 .</param>
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
