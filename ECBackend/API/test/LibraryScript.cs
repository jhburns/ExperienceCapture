namespace Carter.Tests.LibraryScripts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Carter.App.Lib.Authentication;
    using Carter.App.Lib.Environment;
    using Carter.App.Lib.Generate;
    using Carter.App.Lib.Repository;
    using Carter.App.Lib.Timer;

    using Carter.App.Route.Sessions;
    using Carter.App.Route.UsersAndAuthentication;

    using Carter.Tests.HostingExtra;

    using MongoDB.Bson;

    using Moq;

    using Xunit;

    public class TimerTests
    {
        [Fact]
        public void IsNotAfterPositive()
        {
            var date = new BsonDateTime(DateTime.UtcNow);

            Assert.False(date.IsAfter(new DateProvider(), 100000), "IsAfter is true when parameter expirationTime is large and positive.");
        }

        [Fact]
        public void IsAfterNegative()
        {
            var date = new BsonDateTime(DateTime.UtcNow);

            Assert.True(date.IsAfter(new DateProvider(), -100000), "IsAfter is false when parameter expirationTime is negative.");
        }

        [Theory]
        [InlineData(1000, 0)]
        [InlineData(1, 0)]
        [InlineData(10, 1)]
        [InlineData(1000, 100)]
        public void IsAfterFutureTime(int change, int range)
        {
            var fixedDate = DateTime.UtcNow;
            var date = new BsonDateTime(fixedDate);

            var dateMock = new Mock<IDateExtra>();
            dateMock.Setup(d => d.UtcNow)
                .Returns(fixedDate.AddSeconds(change));

            Assert.True(date.IsAfter(dateMock.Object, range), "IsAfter is false when the date is not expired.");
        }

        [Theory]
        [InlineData(0, 1000)]
        [InlineData(0, 1)]
        [InlineData(1, 10)]
        [InlineData(100, 1000)]
        public void IsNotAfterFutureTime(int change, int range)
        {
            var fixedDate = DateTime.UtcNow;
            var date = new BsonDateTime(fixedDate);

            var dateMock = new Mock<IDateExtra>();
            dateMock.Setup(d => d.UtcNow)
                .Returns(fixedDate.AddSeconds(change));

            Assert.False(date.IsAfter(dateMock.Object, range), "IsAfter is true when the date is expired.");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100000000)]
        public void DateIsProjectedForwardCorrectly(int input)
        {
            var fixedDate = DateTime.UtcNow;

            var dateMock = new Mock<IDateExtra>();
            dateMock.Setup(d => d.UtcNow)
                .Returns(fixedDate);

            Assert.True(
                TimerExtra.ProjectSeconds(dateMock.Object, input) > new BsonDateTime(fixedDate),
                "ProjectSeconds is larger when projecting into the future.");
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

    public class AuthenticationTests
    {
        [Theory]
        [InlineData("test")]
        [InlineData("qKLr9R4LXrDLUh6JKh4IL5wXOrrCXOxUNxB81DXAwH8=")]
        [InlineData("")]
        public void HashIsDifferent(string key)
        {
            string hash = PasswordHasher.Hash(key);

            Assert.True(key != hash, "Hash is not different than key");
        }

        [Fact]
        public void HashNullIsNull()
        {
            Assert.True(PasswordHasher.Hash(null) == null, "Hashing null is not null");
        }

        [Theory]
        [InlineData("test")]
        [InlineData("qKLr9R4LXrDLUh6JKh4IL5wXOrrCXOxUNxB81DXAwH8=")]
        [InlineData("")]
        public void HashHasSufficientLength(string key)
        {
            string hash = PasswordHasher.Hash(key);

            Assert.True(hash.Length >= 44, "Hash is smaller than 44 characters.");
        }

        [Theory]
        [InlineData("test")]
        [InlineData("qKLr9R4LXrDLUh6JKh4IL5wXOrrCXOxUNxB81DXAwH8=")]
        [InlineData("")]
        public void HashIsSufficientlyDiscrete(string key)
        {
            var hashes = new List<string>();

            foreach (var t in Enumerable.Range(0, 1000))
            {
                hashes.Add(PasswordHasher.Hash(key));
            }

            bool isSame = hashes.Distinct().Count() == 1;

            Assert.True(isSame, "Hashes are not all the same for a given key.");
        }

        [Theory]
        [InlineData("*")]
        [InlineData("qKLr9R4LXrDLUh6JKh4IL5*wXOrrCXOxUNxB81DXAwH8=")]
        public void IllegalCharactersInKeyIsNull(string key)
        {
            var hash = PasswordHasher.Hash(key);

            Assert.True(hash == null, "Key with illegal characters doesn't result in null hash.");
        }

        [Theory]
        [InlineData("*")]
        [InlineData("qKLr9R4LXrDLUh6JKh4IL5*wXOrrCXOxUNxB81DXAwH8=")]
        public void IllegalCharactersInKeyIsFalse(string key)
        {
            var hash = PasswordHasher.Check("dummy", key);

            Assert.True(hash == false, "Key with illegal characters doesn't result in false check.");
        }

        [Theory]
        [InlineData("test")]
        [InlineData("qKLr9R4LXrDLUh6JKh4IL5wXOrrCXOxUNxB81DXAwH8=")]
        [InlineData("")]
        public void HashIsCancelable(string key)
        {
            bool isCorrect = PasswordHasher.Check(key, PasswordHasher.Hash(key));

            Assert.True(isCorrect, "Hash is not cancelable");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("false")]
        [InlineData("udnSZXxinrbAFQmC1vFyaEWs8hfmrh1NBUO8dhTi")]
        public async void ValidateUserIsNullForDummyToken(string value)
        {
            Environment.SetEnvironmentVariable("admin_password_hash", "1");
            Environment.SetEnvironmentVariable("unsafe_do_no_validate_user", value);
            Environment.SetEnvironmentVariable("gcp_client_id", "3");
            Environment.SetEnvironmentVariable("aws_domain_name", "4");

            var env = ConfigureAppEnvironment.FromEnv();

            var user = await GoogleApi.ValidateUser("dummy", env);

            Assert.True(user == null, "Token failed validation is not null");
        }

        [Fact]
        public async void ValidateUserSkipHasMockId()
        {
            Environment.SetEnvironmentVariable("admin_password_hash", "1");
            Environment.SetEnvironmentVariable("unsafe_do_no_validate_user", "true");
            Environment.SetEnvironmentVariable("gcp_client_id", "3");
            Environment.SetEnvironmentVariable("aws_domain_name", "4");

            var env = ConfigureAppEnvironment.FromEnv();

            var user = await GoogleApi.ValidateUser("dummy", env);

            Assert.True(user.Subject == "123456789109876543210", "Dummy subject when validating is correct.");
        }
    }

    // Note: the following tests are not thread safe
    public class EnvironmentTests
    {
        [Theory]
        [InlineData("test")]
        [InlineData("udnSZXxinrbAFQmC1vFyaEWs8hfmrh1NBUO8dhTi")]
        public void EnvironmentIsCaptured(string value)
        {
            Environment.SetEnvironmentVariable("admin_password_hash", value);
            Environment.SetEnvironmentVariable("unsafe_do_no_validate_user", value);
            Environment.SetEnvironmentVariable("gcp_client_id", value);
            Environment.SetEnvironmentVariable("aws_domain_name", value);

            var env = ConfigureAppEnvironment.FromEnv();

            Assert.True(env.PasswordHash == value, "Password hash env var is not captured from the environment.");
            Assert.True(env.SkipValidation == value, "Skip validation env var is not captured from the environment.");
            Assert.True(env.Audience == value, "Audience env var is not captured from the environment.");
            Assert.True(env.Domain == value, "Domain hash env var is not captured from the environment.");
        }

        [Fact]
        public void EnvironmentIsCapturedDifferently()
        {
            Environment.SetEnvironmentVariable("admin_password_hash", "1");
            Environment.SetEnvironmentVariable("unsafe_do_no_validate_user", "2");
            Environment.SetEnvironmentVariable("gcp_client_id", "3");
            Environment.SetEnvironmentVariable("aws_domain_name", "4");

            var env = ConfigureAppEnvironment.FromEnv();

            Assert.True(env.PasswordHash == "1", "Password hash env var is not captured from the environment.");
            Assert.True(env.SkipValidation == "2", "Skip validation env var is not captured from the environment.");
            Assert.True(env.Audience == "3", "Audience env var is not captured from the environment.");
            Assert.True(env.Domain == "4", "Domain hash env var is not captured from the environment.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void EnvironmentPasswordSetOrThrow(string value)
        {
            Environment.SetEnvironmentVariable("admin_password_hash", value);
            Environment.SetEnvironmentVariable("unsafe_do_no_validate_user", "2");
            Environment.SetEnvironmentVariable("gcp_client_id", "3");
            Environment.SetEnvironmentVariable("aws_domain_name", "4");

            Assert.Throws<EnvironmentVarNotSet>(() =>
            {
                ConfigureAppEnvironment.FromEnv();
            });
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void EnvironmentValidDefaultToFalse(string value)
        {
            Environment.SetEnvironmentVariable("admin_password_hash", "1");
            Environment.SetEnvironmentVariable("unsafe_do_no_validate_user", value);
            Environment.SetEnvironmentVariable("gcp_client_id", "3");
            Environment.SetEnvironmentVariable("aws_domain_name", "4");

            var env = ConfigureAppEnvironment.FromEnv();

            Assert.True(env.SkipValidation == "false", "Skip validation env var is not string false when null.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void EnvironmentAudienceSetOrThrow(string value)
        {
            Environment.SetEnvironmentVariable("admin_password_hash", "1");
            Environment.SetEnvironmentVariable("unsafe_do_no_validate_user", "2");
            Environment.SetEnvironmentVariable("gcp_client_id", value);
            Environment.SetEnvironmentVariable("aws_domain_name", "4");

            Assert.Throws<EnvironmentVarNotSet>(() =>
            {
                ConfigureAppEnvironment.FromEnv();
            });
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void EnvironmentDomainSetOrThrow(string value)
        {
            Environment.SetEnvironmentVariable("admin_password_hash", "1");
            Environment.SetEnvironmentVariable("unsafe_do_no_validate_user", "2");
            Environment.SetEnvironmentVariable("gcp_client_id", "3");
            Environment.SetEnvironmentVariable("aws_domain_name", value);

            Assert.Throws<EnvironmentVarNotSet>(() =>
            {
                ConfigureAppEnvironment.FromEnv();
            });
        }
    }

    public class NetworkTests
    {
        // These tests use arbitrary endpoints for simplicity
        [Fact]
        public async Task StringTypeIsCorrect()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema();
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result);

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Delete, $"/sessions/EXEX");
            var response = await client.SendAsync(request);

            Assert.Equal(
                "text/plain; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task JsonTypeIsCorrect()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema()
                {
                    User = new PersonSchema
                    {
                    },
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Get, "/sessions/EXEX");
            var response = await client.SendAsync(request);

            Assert.Equal(
                "application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task JsonIsUglyCorrect()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema()
                {
                    User = new PersonSchema
                    {
                    },
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Get, "/sessions/EXEX?ugly=true");
            var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal(
                "application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            Assert.False(body.Contains(" "), "Ugly json should not contain spaces.");
            Assert.False(body.Contains("\t"), "Ugly json should not contain tabs.");
            Assert.False(body.Contains("\r"), "Ugly json should not contain carriage return.");
            Assert.False(body.Contains("\n"), "Ugly json should not contain newlines.");
        }

        [Fact]
        public async Task BsonTypeIsCorrect()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema()
                {
                    User = new PersonSchema
                    {
                    },
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Get, "/sessions/EXEX?bson=true");
            var response = await client.SendAsync(request);

            Assert.Equal(
                "application/bson",
                response.Content.Headers.ContentType.ToString());
        }
    }
}
