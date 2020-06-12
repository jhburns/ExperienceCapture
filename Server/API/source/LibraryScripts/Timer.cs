namespace Carter.App.Lib.Timer
{
    using System;
    using MongoDB.Bson;

    public interface IDateExtra
    {
        DateTime Now { get; }
    }

    public static class TimerExtra
    {
        public static bool IsAfter(this BsonDateTime start, IDateExtra date, int expirationTime)
        {
            BsonDateTime endTime = new BsonDateTime(date.Now.AddSeconds(-expirationTime));
            return start.CompareTo(endTime) < 0;
        }

        public static BsonDateTime ProjectSeconds(IDateExtra date, int expirationTime)
        {
            return new BsonDateTime(date.Now.AddSeconds(expirationTime));
        }
    }

    public sealed class DateProvider : IDateExtra
    {
        public DateTime Now
        {
            get => DateTime.Now;
        }
    }
}