namespace Carter.Tests.Route.Users
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Carter.App.Lib.Repository;
    using Carter.App.Lib.Timer;
    using Carter.App.Route.NewSignUp;
    using Carter.App.Route.Users;

    using Carter.Tests.HostingExtra;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;

    using Moq;

    using Xunit;

    public class UsersTests
    {
        [Theory]
        [InlineData("{}")]
        [InlineData("{ \"idToken\": 1234567890 }")]
        [InlineData("{ \"signUpToken\": 1234567890 }")]
        [InlineData("{ \"signUpToken\": \"1234567890\" }")]
        [InlineData("{ \"signUpToken\": \"f\", \"idToken\": 1234567890 }")]
        [InlineData("{ \"signUpToken\": \"\", \"idToken\": \"d\" }")]
        [InlineData("{ \"signUpToken\": \"g\", \"idToken\": \"\" }")]
        public async Task PersonHasToBeValidPostUsers(string input)
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Post, "/users");
            request.Content = new StringContent(input, Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Creating a person is allowed with a poor request.");
        }

        [Fact]
        public async Task WrongTokenIsUnauthorizedPostUsers()
        {
            var signUpMock = new Mock<IRepository<SignUpTokenSchema>>();

            var result = new Task<SignUpTokenSchema>(() =>
            {
                return null;
            });

            var client = CustomHost.Create(signUpMock: signUpMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/users");
            request.Content = new StringContent("{ \"signUpToken\": \"b\", \"idToken\": \"a\" }", Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.Unauthorized,
                "Creating a person is allowed with a poor request.");
        }

        [Theory]
        [InlineData(86401)]
        [InlineData(1000000)]
        public async Task ExpiredTokenIsUnauthorizedPostUsers(int seconds)
        {
            var setTime = new DateProvider().Now;
            var dateMock = new Mock<IDateExtra>();
            dateMock.SetupGet(d => d.Now)
                .Returns(setTime.AddSeconds(seconds))
                .Verifiable("Now was never called.");

            var signUpMock = new Mock<IRepository<SignUpTokenSchema>>();

            var result = new Task<SignUpTokenSchema>(() =>
            {
                return new SignUpTokenSchema
                {
                    CreatedAt = new BsonDateTime(setTime),
                };
            });
            result.Start();

            signUpMock.Setup(s => s.FindOne(It.IsAny<FilterDefinition<SignUpTokenSchema>>()))
                .Returns(result)
                .Verifiable("A sign-up token was not looked for.");

            var client = CustomHost.Create(signUpMock: signUpMock, dateMock: dateMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/users");
            request.Content = new StringContent("{ \"signUpToken\": \"b\", \"idToken\": \"a\" }", Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.Unauthorized,
                "Creating a person is allowed with a poor request.");
        }

        [Fact]
        public async Task ExistingPersonIsConflictPostUsers()
        {
            var setTime = new DateProvider().Now;
            var dateMock = new Mock<IDateExtra>();
            dateMock.SetupGet(d => d.Now)
                .Returns(setTime)
                .Verifiable("Now was never called.");

            var signUpMock = new Mock<IRepository<SignUpTokenSchema>>();

            var result = new Task<SignUpTokenSchema>(() =>
            {
                return new SignUpTokenSchema
                {
                    CreatedAt = new BsonDateTime(setTime.AddSeconds(1)),
                };
            });
            result.Start();

            signUpMock.Setup(s => s.FindOne(It.IsAny<FilterDefinition<SignUpTokenSchema>>()))
                .Returns(result)
                .Verifiable("A sign-up token was not looked for.");

            var personMock = new Mock<IRepository<PersonSchema>>();

            var personResult = new Task<PersonSchema>(() =>
            {
                return new PersonSchema
                {
                };
            });
            personResult.Start();

            personMock.Setup(p => p.FindById(It.IsAny<string>()))
                .Returns(personResult)
                .Verifiable("A person token was not looked for.");

            var client = CustomHost.Create(signUpMock: signUpMock, dateMock: dateMock, personMock: personMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/users");
            request.Content = new StringContent("{ \"signUpToken\": \"b\", \"idToken\": \"a\" }", Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            Console.WriteLine(response.StatusCode);
            Assert.True(
                response.StatusCode == HttpStatusCode.Conflict,
                "User creation does not return a conflict.");
        }

        [Fact]
        public async Task AddIsCalledPostUsers()
        {
            var setTime = new DateProvider().Now;
            var dateMock = new Mock<IDateExtra>();
            dateMock.SetupGet(d => d.Now)
                .Returns(setTime)
                .Verifiable("Now was never called.");

            var signUpMock = new Mock<IRepository<SignUpTokenSchema>>();

            var result = new Task<SignUpTokenSchema>(() =>
            {
                return new SignUpTokenSchema
                {
                    CreatedAt = new BsonDateTime(setTime.AddSeconds(1)),
                };
            });
            result.Start();

            signUpMock.Setup(s => s.FindOne(It.IsAny<FilterDefinition<SignUpTokenSchema>>()))
                .Returns(result)
                .Verifiable("A sign-up token was not looked for.");

            var personMock = new Mock<IRepository<PersonSchema>>();

            var personResult = new Task<PersonSchema>(() =>
            {
                return null;
            });
            personResult.Start();

            personMock.Setup(p => p.FindById(It.IsAny<string>()))
                .Returns(personResult)
                .Verifiable("A person token was not looked for.");

            personMock.Setup(p => p.Add(It.IsAny<PersonSchema>()))
                .Verifiable();

            var client = CustomHost.Create(signUpMock: signUpMock, dateMock: dateMock, personMock: personMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/users");
            request.Content = new StringContent("{ \"signUpToken\": \"b\", \"idToken\": \"a\" }", Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("")]
        [InlineData("/")]
        [InlineData("?")]
        [InlineData("/?")]
        [InlineData("/?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task MultipulRoutesAreOkPostUsers(string input)
        {
            var setTime = new DateProvider().Now;
            var dateMock = new Mock<IDateExtra>();
            dateMock.SetupGet(d => d.Now)
                .Returns(setTime)
                .Verifiable("Now was never called.");

            var signUpMock = new Mock<IRepository<SignUpTokenSchema>>();

            var result = new Task<SignUpTokenSchema>(() =>
            {
                return new SignUpTokenSchema
                {
                    CreatedAt = new BsonDateTime(setTime.AddSeconds(1)),
                };
            });
            result.Start();

            signUpMock.Setup(s => s.FindOne(It.IsAny<FilterDefinition<SignUpTokenSchema>>()))
                .Returns(result)
                .Verifiable("A sign-up token was not looked for.");

            var personMock = new Mock<IRepository<PersonSchema>>();

            var personResult = new Task<PersonSchema>(() =>
            {
                return null;
            });
            personResult.Start();

            personMock.Setup(p => p.FindById(It.IsAny<string>()))
                .Returns(personResult)
                .Verifiable("A person token was not looked for.");

            var client = CustomHost.Create(signUpMock: signUpMock, dateMock: dateMock, personMock: personMock);

            var request = CustomRequest.Create(HttpMethod.Post, $"/users{input}");
            request.Content = new StringContent("{ \"signUpToken\": \"b\", \"idToken\": \"a\" }", Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.True(body == "OK", "Creating a new person is not 'OK'.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("/")]
        [InlineData("?")]
        [InlineData("/?")]
        [InlineData("/?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task OtherMethodsUsers(string input)
        {
            var client = CustomHost.Create();

            var requestPut = CustomRequest.Create(HttpMethod.Put, $"/users{input}");
            var responsePut = await client.SendAsync(requestPut);

            Assert.True(
                responsePut.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Putting users is an allowed method.");

            var requestPatch = CustomRequest.Create(HttpMethod.Patch, $"/users{input}");
            var responsePatch = await client.SendAsync(requestPatch);

            Assert.True(
                responsePatch.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Patching users is an allowed method.");

            var requestGet = CustomRequest.Create(HttpMethod.Get, $"/users{input}");
            var responseGet = await client.SendAsync(requestGet);

            Assert.True(
                responseGet.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Gettings users is an allowed method.");

            var requestDelete = CustomRequest.Create(HttpMethod.Delete, $"/users{input}");
            var responseDelete = await client.SendAsync(requestDelete);

            Assert.True(
                responseDelete.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Deleting users is an allowed method.");
        }
    }
}