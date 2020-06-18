namespace Carter.App.Lib.Generate
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;

    public class Generate
    {
        internal static readonly string LettersForId = "ABCEFGHJKLMNPQRUVWXY";
        internal static readonly string NumbersForId = "346789";

        private static Random random = new Random();

        // Should NOT be considered secure
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

        // Should be considered secure
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