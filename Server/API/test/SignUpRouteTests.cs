namespace Carter.Tests.Route.PreSecurity
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Carter.App.Lib.Repository;
    using Carter.App.Route.NewSignUp;
    using Carter.App.Route.Users;

    using Carter.Tests.HostingExtra;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;

    using Moq;

    using Xunit;

    public class SignUpTests
    {
        [Fact]
        public async Task RequiresAccessPostSignUp()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Post, "/users/signUp", false);
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Creating a sign-up token is a bad request without access token.");
        }

        [Fact]
        public async Task AddIsCalledPostSignUp()
        {
            var accessMock = new Mock<IRepository<AccessTokenSchema>>();

            var result = new Task<AccessTokenSchema>(() =>
            {
                return new AccessTokenSchema
                {
                    InternalId = ObjectId.GenerateNewId(),
                    Hash = string.Empty,
                    User = ObjectId.GenerateNewId(),

                    // A day so the token can't expire while running
                    CreatedAt = new BsonDateTime(DateTime.Now.AddSeconds(86400)),
                };
            });
            result.Start();

            accessMock.Setup(a => a.FindOne(It.IsAny<FilterDefinition<AccessTokenSchema>>()))
                .Returns(result);

            accessMock.Setup(s => s.Add(It.IsAny<AccessTokenSchema>()))
                .Verifiable("An access token was never added.");

            var client = CustomHost.Create(accessMock: accessMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/users/signUp");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ResponceIsValidPostSignUp()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Post, "/users/signUp");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var data = BsonSerializer.Deserialize<SignUpTokenResponce>(body);

            Assert.False(data == null, "SignUp data is null.");
            Assert.False(data.SignUpToken == null, "SignUp token data is null.");
            Assert.False(data.SignUpToken == string.Empty, "SignUp token data is empty.");
            Assert.False(data.Expiration == null, "SignUp expiration data is null.");
        }

        [Theory]
        [InlineData("?")]
        [InlineData("/?")]
        [InlineData("/?sdfsdf=sdfdsf34543&")]
        public async Task ResponceIsValidBsonPostSignUp(string input)
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Post, $"/users/signUp{input}bson=true");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsByteArrayAsync();
            var data = BsonSerializer.Deserialize<SignUpTokenResponce>(body);

            Assert.False(data == null, "SignUp data is null.");
            Assert.False(data.SignUpToken == null, "SignUp token data is null.");
            Assert.False(data.SignUpToken == string.Empty, "SignUp token data is empty.");
            Assert.False(data.Expiration == null, "SignUp expiration data is null.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("/")]
        [InlineData("?")]
        [InlineData("/?")]
        [InlineData("/?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task MultipleRoutesPostSignUp(string input)
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Post, $"/users/signUp{input}");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ResponceExpirationIsInTheFuturePostSignUp()
        {
            var now = new BsonDateTime(DateTime.Now);
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Post, "/users/signUp");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            var data = BsonSerializer.Deserialize<SignUpTokenResponce>(body);

            Assert.True(data.Expiration > now, "SignUp expiration data is before the current time.");
        }

        [Fact]
        public async Task ResponceTokensAreDifferentPostSignUp()
        {
            var client = CustomHost.Create();

            var requestFirst = CustomRequest.Create(HttpMethod.Post, "/users/signUp");
            var responseFirst = await client.SendAsync(requestFirst);
            responseFirst.EnsureSuccessStatusCode();

            var bodyFirst = await responseFirst.Content.ReadAsStringAsync();

            var dataFirst = BsonSerializer.Deserialize<SignUpTokenResponce>(bodyFirst);

            var requestSecond = CustomRequest.Create(HttpMethod.Post, "/users/signUp");
            var responseSecond = await client.SendAsync(requestSecond);
            responseFirst.EnsureSuccessStatusCode();

            var bodySecond = await responseSecond.Content.ReadAsStringAsync();

            var dataSecond = BsonSerializer.Deserialize<SignUpTokenResponce>(bodySecond);

            Assert.True(dataFirst.SignUpToken != dataSecond.SignUpToken, "SignUp tokens are the same.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("/")]
        [InlineData("?")]
        [InlineData("/?")]
        [InlineData("/?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task OtherMethodsTag(string input)
        {
            var client = CustomHost.Create();

            var requestPut = CustomRequest.Create(HttpMethod.Put, $"/users/signUp{input}");
            var responsePut = await client.SendAsync(requestPut);

            Assert.True(
                responsePut.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Putting tags is an allowed method.");

            var requestPatch = CustomRequest.Create(HttpMethod.Put, $"/users/signUp{input}");
            var responsePatch = await client.SendAsync(requestPatch);

            Assert.True(
                responsePatch.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Patching tags is an allowed method.");

            var requestGet = CustomRequest.Create(HttpMethod.Put, $"/users/signUp{input}");
            var responseGet = await client.SendAsync(requestGet);

            Assert.True(
                responseGet.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Gettings tags is an allowed method.");

            var requestDelete = CustomRequest.Create(HttpMethod.Put, $"/users/signUp{input}");
            var responseDelete = await client.SendAsync(requestDelete);

            Assert.True(
                responseGet.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Deleting tags is an allowed method.");
        }
    }
}