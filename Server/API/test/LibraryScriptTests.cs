using System;

using Carter.App.Lib.Timer;

using MongoDB.Bson;

using Xunit;
 

namespace Carter.Tests.LibraryScripts
{
    public class Timer
    {
        [Fact]
        public void IsNotAfterPositive()
        {
            var date = new BsonDateTime(DateTime.Now);

            Assert.False(date.IsAfter(100000), "IsAfter is true when parameter expirationTime is large and positive.");
        }

        [Fact]
        public void IsAfterNegative()
        {
            var date = new BsonDateTime(DateTime.Now);

            Assert.True(date.IsAfter(-100000), "IsAfter is false when parameter expirationTime is negative.");
        }
    }
}
