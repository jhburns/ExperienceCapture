namespace Carter.App.Lib.Timer
{
    using System;
    using MongoDB.Bson;

    /// <summary>
    /// Wrapper around system time.
    /// </summary>
    public interface IDateExtra
    {
        /// <summary>The UTC value according to the system.</summary>
        DateTime UtcNow { get; }
    }

    /// <summary>
    /// Helpers for time.
    /// </summary>
    public static class TimerExtra
    {
        /// <summary>
        /// Checks whether a time has expired.
        /// </summary>
        /// <returns>
        /// True if the start date and offset combined is after the current date.
        /// </returns>
        /// <param name="start">When to start counting from.</param>
        /// <param name="date">Provides the current date.</param>
        /// <param name="expirationTime">An offset to be added on.</param>
        public static bool IsAfter(this BsonDateTime start, IDateExtra date, int expirationTime)
        {
            BsonDateTime endTime = new BsonDateTime(date.UtcNow.AddSeconds(-expirationTime));
            return start.CompareTo(endTime) < 0;
        }

        /// <summary>
        /// Adds seconds to the current time.
        /// </summary>
        /// <returns>
        /// A projected date in the future.
        /// </returns>
        /// <param name="date">Provides the current date.</param>
        /// <param name="expirationTime">An offset to be added on.</param>
        public static BsonDateTime ProjectSeconds(IDateExtra date, int expirationTime)
        {
            return new BsonDateTime(date.UtcNow.AddSeconds(expirationTime));
        }
    }

    /// <summary>
    /// Implements IDateExtra.
    /// </summary>
    public sealed class DateProvider : IDateExtra
    {
        /// <inheritdoc />
        public DateTime UtcNow
        {
            get => DateTime.UtcNow;
        }
    }
}