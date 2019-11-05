namespace Nancy.App.Session
{
    using System.Collections.Generic;

    public class StoreSession
    {
        private static List<string> ids = new List<string>();

        public static List<string> GetSessions()
        {
            return ids;
        }

        public static void SaveSessions(List<string> sessions)
        {
            ids = sessions;
        }
    }
}