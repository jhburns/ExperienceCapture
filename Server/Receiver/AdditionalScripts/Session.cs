namespace Nancy.App.Session
{
    using System.Collections.Generic;

    public class StoreSession {

        public static List<string> ids = new List<string>();

        public static List<string> getSessions()
        {
            return ids;
        }

        public static void saveSessions(List<string> sessions)
        {
            ids = sessions;
        }
    }
}