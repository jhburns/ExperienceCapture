namespace Nancy.App.Random
{
    using System;
    using System.Collections;
    using System.Linq;

    public class Generate
    {
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCEFGHJKLMNPQRSUVWXY3456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
