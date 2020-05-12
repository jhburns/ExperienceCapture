namespace Carter.Tests.LibraryScripts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Carter.App.Lib.Generate;
    using Carter.App.Lib.Timer;

    using MongoDB.Bson;

    using Xunit;

    public class TimerTests
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

    public class GenerateTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(50000)]
        public void IdIsLength(int length)
        {
            string id = Generate.GetRandomId(length);

            Assert.True(id.Length == length, "The generated id has a different length than provided.");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-5555)]
        public void IdLengthCantBeNegative(int length)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Generate.GetRandomId(length);
            });
        }

        [Fact]
        public void IdDoesNotContainCertainCharacters()
        {
            var illegals = new List<string>() { "D", "I", "O", "S", "T", "Z", "0", "1", "2", "5" };
            bool isContaining = false;

            foreach (var t in Enumerable.Range(0, 1000))
            {
                string id = Generate.GetRandomId(100);

                if (illegals.Any(c => id.Contains(c)))
                {
                    isContaining = true;
                }
            }

            Assert.False(isContaining, "An ID contains an illegal character.");
        }

        [Fact]
        public void IdDoesNotContainLowercase()
        {
            bool isContaining = false;

            foreach (var t in Enumerable.Range(0, 1000))
            {
                string id = Generate.GetRandomId(100);

                if (id.Any(char.IsLower))
                {
                    isContaining = true;
                }
            }

            Assert.False(isContaining, "An ID contains a lowercase character.");
        }

        [Fact]
        public void IdDoesNotContainSpecialCharacters()
        {
            bool isContaining = false;

            foreach (var t in Enumerable.Range(0, 1000))
            {
                string id = Generate.GetRandomId(100);

                if (!id.Any(char.IsLetterOrDigit))
                {
                    isContaining = true;
                }
            }

            Assert.False(isContaining, "An ID contains a special character.");
        }

        [Fact]
        public void IdsAreUnique()
        {
            var ids = new List<string>();

            foreach (var t in Enumerable.Range(0, 1000))
            {
                ids.Add(Generate.GetRandomId(100));
            }

            bool isUnique = ids.Distinct().Count() == ids.Count();
            Assert.True(isUnique, "IDs are not unique.");
        }

        [Fact]
        public void TokenHasSufficientLength()
        {
            var token = Generate.GetRandomToken();
            Assert.True(token.Length >= 44, "Token length is smaller than 44 characters.");
        }

        [Fact]
        public void TokenLengthIsNotExcessive()
        {
            var token = Generate.GetRandomToken();
            Assert.True(token.Length < 1000, "Token length is larger than 1000 characters.");
        }

        [Fact]
        public void TokensAreUnique()
        {
            var tokens = new List<string>();

            foreach (var t in Enumerable.Range(0, 1000))
            {
                tokens.Add(Generate.GetRandomToken());
            }

            bool isUnique = tokens.Distinct().Count() == tokens.Count();
            Assert.True(isUnique, "Tokens are not unique.");
        }
    }
}
