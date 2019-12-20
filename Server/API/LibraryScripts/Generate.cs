namespace Carter.App.Generate
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public class Generate
    {
        internal static readonly string CharsForId = "ABCEFGHJKLMNPQRSUVWXY3456789";
        internal static readonly char[] CharsForToken =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

        private static Random random = new Random();

        // Should NOT be considered secure
        public static string GetRandomId(int length)
        {
            return new string(Enumerable.Repeat(CharsForId, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Taken from here: https://stackoverflow.com/a/1344255
        // Should be considered secure
        public static string GetRandomToken(int size)
        {
            byte[] data = new byte[4 * size];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }

            StringBuilder result = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % CharsForToken.Length;

                result.Append(CharsForToken[idx]);
            }

            return result.ToString();
        }
    }
}