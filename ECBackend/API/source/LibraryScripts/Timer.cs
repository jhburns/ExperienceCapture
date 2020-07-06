namespace Carter.App.Lib.Timer
{
    using System;
    using MongoDB.Bson;

    public interface IDateExtra
    {
        DateTime UtcNow { get; }
    }

    public static class TimerExtra
    {
        public static bool IsAfter(this BsonDateTime start, IDateExtra date, int expirationTime)
        {
            BsonDateTime endTime = new BsonDateTime(date.UtcNow.AddSeconds(-expirationTime));
            return start.CompareTo(endTime) < 0;
        }

        public static BsonDateTime ProjectSeconds(IDateExtra date, int expirationTime)
        {
            return new BsonDateTime(date.UtcNow.AddSeconds(expirationTime));
        }
    }

    public sealed class DateProvider : IDateExtra
    {
        public DateTime UtcNow
        {
            get => DateTime.UtcNow;
        }
    }
}