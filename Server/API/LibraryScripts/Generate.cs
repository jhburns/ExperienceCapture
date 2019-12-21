namespace Carter.App.Lib.Generate
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public class Generate
    {
        internal static readonly string CharsForId = "ABCEFGHJKLMNPQRSUVWXY3456789";

        private static Random random = new Random();

        // Should NOT be considered secure
        public static string GetRandomId(int length)
        {
            return new string(Enumerable.Repeat(CharsForId, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
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