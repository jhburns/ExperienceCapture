namespace Carter.App.Lib.Timer
{
    using System;
    using MongoDB.Bson;

    public static class CheckExpire
    {
        public static bool IsAfter(this BsonDateTime start, int expirationTime)
        {
            BsonDateTime endTime = new BsonDateTime(DateTime.Now.AddSeconds(-expirationTime));
            return start.CompareTo(endTime) < 0;
        }
    }
}