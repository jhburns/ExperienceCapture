namespace Carter.App.Lib.Generate
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;

    /// <summary>
    /// Generates various tokens.
    /// </summary>
    public class Generate
    {
        internal static readonly string LettersForId = "ABCEFGHJKLMNPQRUVWXY";
        internal static readonly string NumbersForId = "346789";

        private static Random random = new Random();

        /// <summary>
        /// Creates an id, for humans. Should NOT be considered secure.
        /// </summary>
        /// <returns>
        /// An id.
        /// </returns>
        /// <param name="length">How long to make the id, cannot be zero or negative.</param>
        public static string GetRandomId(int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length must be positive.");
            }
            else if (length == 0)
            {
                return string.Empty;
            }
            else if (length == 1)
            {
                return new string(Enumerable.Repeat(LettersForId, length)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            }
            else
            {
                var first = new string(Enumerable.Repeat(LettersForId, 1)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                var second = new string(Enumerable.Repeat(LettersForId + NumbersForId, length - 1)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                return first + second;
            }
        }

        /// <summary>
        /// Creates a token, that should be considered to be secure.
        /// </summary>
        /// <returns>
        /// A token, base64 encoded.
        /// </returns>
        public static string GetRandomToken()
        {
            var key = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(key);
                string apiKey = Convert.ToBase64String(key);
                return apiKey;
            }
        }
    }
}