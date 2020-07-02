namespace Carter.Tests.Route.Users
{
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Carter.App.Lib.Environment;
    using Carter.App.Lib.Repository;
    using Carter.App.Lib.Timer;
    using Carter.App.Route.ProtectedUsers;
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
        [InlineData("{ sdfwef2r23, sfdsf3, [], dsfr32ee32ed3d.ewdsfew. 433}")]
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
            var setTime = new DateProvider().UtcNow;
            var dateMock = new Mock<IDateExtra>();
            dateMock.SetupGet(d => d.UtcNow)
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
            var setTime = new DateProvider().UtcNow;
            var dateMock = new Mock<IDateExtra>();
            dateMock.SetupGet(d => d.UtcNow)
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

            Assert.True(
                response.StatusCode == HttpStatusCode.Conflict,
                "User creation does not return a conflict.");
        }

        [Fact]
        public async Task AddIsCalledPostUsers()
        {
            var setTime = new DateProvider().UtcNow;
            var dateMock = new Mock<IDateExtra>();
            dateMock.SetupGet(d => d.UtcNow)
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
            var setTime = new DateProvider().UtcNow;
            var dateMock = new Mock<IDateExtra>();
            dateMock.SetupGet(d => d.UtcNow)
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

        [Fact]
        public async Task MissingUserIsNotFoundPostAccessToken()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Post, "/users/123456789109876543210");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.NotFound,
                "Creating an access token with a missing person is not 'not found'.");
        }

        [Fact]
        public async Task MissingPersonIsNotFoundPostAccessToken()
        {
            var personMock = new Mock<IRepository<PersonSchema>>();

            var result = new Task<PersonSchema>(() =>
            {
                return null;
            });
            result.Start();

            personMock.Setup(p => p.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A person was not looked for.");

            var client = CustomHost.Create(personMock: personMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/users/123456789109876543210/tokens");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.NotFound,
                "Creating an access token with a missing person is not 'not found'.");
        }

        [Theory]
        [InlineData("{}")]
        [InlineData("{ \"idToken\": 1234567890 }")]
        [InlineData("{ \"claimToken\": \"1234567890\" }")]
        [InlineData("{ sdfwef2r23, sfdsf3, [], dsfr32ee32ed3d.ewdsfew. 433}")]
        public async Task RejectsInvalidBodyPostAccessToken(string input)
        {
            var personMock = new Mock<IRepository<PersonSchema>>();

            var result = new Task<PersonSchema>(() =>
            {
                return new PersonSchema
                {
                };
            });
            result.Start();

            personMock.Setup(p => p.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A person was not looked for.");

            var client = CustomHost.Create(personMock: personMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/users/123456789109876543210/tokens");
            request.Content = new StringContent(input, Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Creating an access token with bad schema is not a bad request.");
        }

        [Fact]
        public async Task MismatchingPersonIsConflictPostAccessToken()
        {
            var personMock = new Mock<IRepository<PersonSchema>>();

            var result = new Task<PersonSchema>(() =>
            {
                return new PersonSchema
                {
                };
            });
            result.Start();

            personMock.Setup(p => p.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A person was not looked for.");

            var client = CustomHost.Create(personMock: personMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/users/thisIsWrong/tokens");
            var requestBody = "{ \"idToken\": \"a\" }";
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.Conflict,
                "Creating an access token with a mismatching person is not a conflict.");
        }

        [Fact]
        public async Task AddIsCalledPostAccessToken()
        {
            var personMock = new Mock<IRepository<PersonSchema>>();

            var result = new Task<PersonSchema>(() =>
            {
                return new PersonSchema
                {
                };
            });
            result.Start();

            personMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A person was not looked for.");

            var accessMock = new Mock<IRepository<AccessTokenSchema>>();

            accessMock.Setup(a => a.Add(It.IsAny<AccessTokenSchema>()))
                .Verifiable("An access token was never added.");

            var client = CustomHost.Create(personMock: personMock, accessMock: accessMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/users/123456789109876543210/tokens");
            var requestBody = "{ \"idToken\": \"a\" }";
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ResponceIsValidPostAccessToken()
        {
            var personMock = new Mock<IRepository<PersonSchema>>();

            var result = new Task<PersonSchema>(() =>
            {
                return new PersonSchema
                {
                };
            });
            result.Start();

            personMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A person was not looked for.");

            var accessMock = new Mock<IRepository<AccessTokenSchema>>();

            accessMock.Setup(a => a.Add(It.IsAny<AccessTokenSchema>()))
                .Verifiable("An access token was never added.");

            var client = CustomHost.Create(personMock: personMock, accessMock: accessMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/users/123456789109876543210/tokens");
            var requestBody = "{ \"idToken\": \"a\" }";
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var data = BsonSerializer.Deserialize<AccessTokenResponce>(body);

            Assert.False(data.AccessToken == null, "The access token data is null.");
            Assert.False(data.Expiration == null, "The access token expiration data is null.");
        }

        [Fact]
        public async Task ResponceIsValidBsonPostAccessToken()
        {
            var personMock = new Mock<IRepository<PersonSchema>>();

            var result = new Task<PersonSchema>(() =>
            {
                return new PersonSchema
                {
                };
            });
            result.Start();

            personMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A person was not looked for.");

            var accessMock = new Mock<IRepository<AccessTokenSchema>>();

            accessMock.Setup(a => a.Add(It.IsAny<AccessTokenSchema>()))
                .Verifiable("An access token was never added.");

            var client = CustomHost.Create(personMock: personMock, accessMock: accessMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/users/123456789109876543210/tokens?bson=true");
            var requestBody = "{ \"idToken\": \"a\" }";
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsByteArrayAsync();
            var data = BsonSerializer.Deserialize<AccessTokenResponce>(body);

            Assert.False(data.AccessToken == null, "The access token data is null.");
            Assert.False(data.AccessToken == string.Empty, "The access token data is empty.");
            Assert.False(data.Expiration == null, "The access token expiration data is null.");
        }

        [Fact]
        public async Task MissingClaimIsUnauthorizedPostAccessToken()
        {
            var personMock = new Mock<IRepository<PersonSchema>>();

            var result = new Task<PersonSchema>(() =>
            {
                return new PersonSchema
                {
                };
            });
            result.Start();

            personMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A person was not looked for.");

            var claimMock = new Mock<IRepository<ClaimTokenSchema>>();

            var client = CustomHost.Create(personMock: personMock, claimMock: claimMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/users/123456789109876543210/tokens");
            var requestBody = "{ \"idToken\": \"a\", \"claimToken\": \"b\" }";
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.Unauthorized,
                "A missing claim is not unauthorized.");
        }

        [Fact]
        public async Task ThingsAreCalledForClaimPostAccessToken()
        {
            var setTime = new DateProvider().UtcNow;
            var dateMock = new Mock<IDateExtra>();
            dateMock.SetupGet(d => d.UtcNow)
                .Returns(setTime)
                .Verifiable("Now was never called.");

            var personMock = new Mock<IRepository<PersonSchema>>();

            var result = new Task<PersonSchema>(() =>
            {
                return new PersonSchema
                {
                };
            });
            result.Start();

            personMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A person was not looked for.");

            var claimMock = new Mock<IRepository<ClaimTokenSchema>>();

            var claimResult = new Task<ClaimTokenSchema>(() =>
            {
                return new ClaimTokenSchema
                {
                    IsExisting = true,
                    Access = null,
                    CreatedAt = new BsonDateTime(setTime),
                };
            });
            claimResult.Start();

            claimMock.Setup(s => s.FindOne(It.IsAny<FilterDefinition<ClaimTokenSchema>>()))
                .Returns(claimResult)
                .Verifiable("A claim was not looked for.");

            claimMock.Setup(s => s.Update(
                    It.IsAny<FilterDefinition<ClaimTokenSchema>>(),
                    It.IsAny<UpdateDefinition<ClaimTokenSchema>>()))
                .Verifiable("A claim is not updated.");

            var client = CustomHost.Create(personMock: personMock, claimMock: claimMock, dateMock: dateMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/users/123456789109876543210/tokens");
            var requestBody = "{ \"idToken\": \"a\", \"claimToken\": \"b\" }";
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ResponceIsOkPostAccessToken()
        {
            var setTime = new DateProvider().UtcNow;
            var dateMock = new Mock<IDateExtra>();
            dateMock.SetupGet(d => d.UtcNow)
                .Returns(setTime)
                .Verifiable("Now was never called.");

            var personMock = new Mock<IRepository<PersonSchema>>();

            var result = new Task<PersonSchema>(() =>
            {
                return new PersonSchema
                {
                };
            });
            result.Start();

            personMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A person was not looked for.");

            var claimMock = new Mock<IRepository<ClaimTokenSchema>>();

            var claimResult = new Task<ClaimTokenSchema>(() =>
            {
                return new ClaimTokenSchema
                {
                    IsExisting = false,
                    CreatedAt = new BsonDateTime(setTime),
                };
            });
            claimResult.Start();

            claimMock.Setup(s => s.FindOne(It.IsAny<FilterDefinition<ClaimTokenSchema>>()))
                .Returns(claimResult)
                .Verifiable("A claim was not looked for.");

            claimMock.Setup(s => s.Update(
                    It.IsAny<FilterDefinition<ClaimTokenSchema>>(),
                    It.IsAny<UpdateDefinition<ClaimTokenSchema>>()))
                .Verifiable("A claim is not updated.");

            var client = CustomHost.Create(personMock: personMock, claimMock: claimMock, dateMock: dateMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/users/123456789109876543210/tokens");
            var requestBody = "{ \"idToken\": \"a\", \"claimToken\": \"b\" }";
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.True(
                body == "OK",
                "An claim token being filled does not have a responce of 'OK'.");
        }

        [Fact]
        public async Task ExpiredTokenIsUnauthorizedPostAccessToken()
        {
            var setTime = new DateProvider().UtcNow;
            var dateMock = new Mock<IDateExtra>();
            dateMock.SetupGet(d => d.UtcNow)
                .Returns(setTime.AddSeconds(3601))
                .Verifiable("Now was never called.");

            var personMock = new Mock<IRepository<PersonSchema>>();

            var result = new Task<PersonSchema>(() =>
            {
                return new PersonSchema
                {
                };
            });
            result.Start();

            personMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A person was not looked for.");

            var claimMock = new Mock<IRepository<ClaimTokenSchema>>();

            var claimResult = new Task<ClaimTokenSchema>(() =>
            {
                return new ClaimTokenSchema
                {
                    IsExisting = false,
                    CreatedAt = new BsonDateTime(setTime),
                };
            });
            claimResult.Start();

            claimMock.Setup(s => s.FindOne(It.IsAny<FilterDefinition<ClaimTokenSchema>>()))
                .Returns(claimResult)
                .Verifiable("A claim was not looked for.");

            claimMock.Setup(s => s.Update(
                    It.IsAny<FilterDefinition<ClaimTokenSchema>>(),
                    It.IsAny<UpdateDefinition<ClaimTokenSchema>>()))
                .Verifiable("A claim is not updated.");

            var client = CustomHost.Create(personMock: personMock, claimMock: claimMock, dateMock: dateMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/users/123456789109876543210/tokens");
            var requestBody = "{ \"idToken\": \"a\", \"claimToken\": \"b\" }";
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.Unauthorized,
                "An expired claim token being filled is not unauthorized.");
        }

        [Theory]
        [InlineData("123456789109876543210/tokens")]
        [InlineData("123456789109876543210/tokens/")]
        [InlineData("123456789109876543210/tokens?")]
        [InlineData("123456789109876543210/tokens/?")]
        [InlineData("123456789109876543210/tokens/?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task OtherMethodsAccessTokens(string input)
        {
            var client = CustomHost.Create();

            var requestPut = CustomRequest.Create(HttpMethod.Put, $"/users/{input}");
            var responsePut = await client.SendAsync(requestPut);

            Assert.True(
                responsePut.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Putting access tokens is an allowed method.");

            var requestPatch = CustomRequest.Create(HttpMethod.Patch, $"/users/{input}");
            var responsePatch = await client.SendAsync(requestPatch);

            Assert.True(
                responsePatch.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Patching access tokens is an allowed method.");

            var requestGet = CustomRequest.Create(HttpMethod.Get, $"/users/{input}");
            var responseGet = await client.SendAsync(requestGet);

            Assert.True(
                responseGet.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Gettings access tokens is an allowed method.");

            var requestDelete = CustomRequest.Create(HttpMethod.Delete, $"/users/{input}");
            var responseDelete = await client.SendAsync(requestDelete);

            Assert.True(
                responseDelete.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Deleting access tokens is an allowed method.");
        }

        [Fact]
        public async Task AddIsCalledPostClaim()
        {
            var claimMock = new Mock<IRepository<ClaimTokenSchema>>();

            var result = new Task<ClaimTokenSchema>(() =>
            {
                return new ClaimTokenSchema
                {
                };
            });
            result.Start();

            claimMock.Setup(s => s.Add(It.IsAny<ClaimTokenSchema>()))
                .Returns(result)
                .Verifiable("A claim token is not added.");

            var client = CustomHost.Create(claimMock: claimMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/users/claims/");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ResponceIsValidPostClaim()
        {
            var claimMock = new Mock<IRepository<ClaimTokenSchema>>();

            var result = new Task<ClaimTokenSchema>(() =>
            {
                return new ClaimTokenSchema
                {
                };
            });
            result.Start();

            claimMock.Setup(s => s.Add(It.IsAny<ClaimTokenSchema>()))
                .Returns(result)
                .Verifiable("A claim token is not added.");

            var client = CustomHost.Create(claimMock: claimMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/users/claims");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var data = BsonSerializer.Deserialize<ClaimTokenResponce>(body);

            Assert.False(data.ClaimToken == null, "Claim token data is null.");
            Assert.False(data.ClaimToken == string.Empty, "Claim token data is empty.");
            Assert.False(data.Expiration == null, "Claim token expiration data is empty.");
        }

        [Fact]
        public async Task ResponceIsValidBsonPostClaim()
        {
            var claimMock = new Mock<IRepository<ClaimTokenSchema>>();

            var result = new Task<ClaimTokenSchema>(() =>
            {
                return new ClaimTokenSchema
                {
                };
            });
            result.Start();

            claimMock.Setup(s => s.Add(It.IsAny<ClaimTokenSchema>()))
                .Returns(result)
                .Verifiable("A claim token is not added.");

            var client = CustomHost.Create(claimMock: claimMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/users/claims?bson=true");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsByteArrayAsync();
            var data = BsonSerializer.Deserialize<ClaimTokenResponce>(body);

            Assert.False(data.ClaimToken == null, "Claim token data is null.");
            Assert.False(data.ClaimToken == string.Empty, "Claim token data is empty.");
            Assert.False(data.Expiration == null, "Claim token expiration data is empty.");
        }

        [Fact]
        public async Task MissingClaimIsNotFoundGetClaims()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Get, "/users/claims");
            request.Headers.TryAddWithoutValidation("Cookie", "ExperienceCapture-Claim-Token=" + "ok");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.NotFound,
                "Getting a missing claim is not 'not found'.");
        }

        [Fact]
        public async Task NonExistingClaimIsNotFoundGetClaims()
        {
            var claimMock = new Mock<IRepository<ClaimTokenSchema>>();

            var result = new Task<ClaimTokenSchema>(() =>
            {
                return new ClaimTokenSchema
                {
                    IsExisting = false,
                };
            });
            result.Start();

            claimMock.Setup(s => s.FindOne(It.IsAny<FilterDefinition<ClaimTokenSchema>>()))
                .Returns(result)
                .Verifiable("A claim token is not searched for.");

            var client = CustomHost.Create(claimMock: claimMock);

            var request = CustomRequest.Create(HttpMethod.Get, "/users/claims");
            request.Headers.TryAddWithoutValidation("Cookie", "ExperienceCapture-Claim-Token=" + "ok");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.NotFound,
                "Getting a non-existing claim token is not 'not found'.");
        }

        [Fact]
        public async Task RequiresCookiePostClaims()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Get, "/users/claims");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Getting a claim token without a cookie is not a bad request.");
        }

        [Fact]
        public async Task ExpiredClaimIsNotFoundGetClaims()
        {
            var setTime = new DateProvider().UtcNow;
            var dateMock = new Mock<IDateExtra>();
            dateMock.SetupGet(d => d.UtcNow)
                .Returns(setTime.AddSeconds(3601))
                .Verifiable("Now was never called.");

            var claimMock = new Mock<IRepository<ClaimTokenSchema>>();

            var result = new Task<ClaimTokenSchema>(() =>
            {
                return new ClaimTokenSchema
                {
                    IsExisting = true,
                    CreatedAt = new BsonDateTime(setTime),
                };
            });
            result.Start();

            claimMock.Setup(s => s.FindOne(It.IsAny<FilterDefinition<ClaimTokenSchema>>()))
                .Returns(result)
                .Verifiable("A claim token is not searched for.");

            var client = CustomHost.Create(claimMock: claimMock, dateMock: dateMock);

            var request = CustomRequest.Create(HttpMethod.Get, "/users/claims");
            request.Headers.TryAddWithoutValidation("Cookie", "ExperienceCapture-Claim-Token=" + "ok");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.NotFound,
                "Getting an expired claim token is not 'not found'.");
        }

        [Fact]
        public async Task NotAccessIsPendingGetClaims()
        {
            var setTime = new DateProvider().UtcNow;
            var dateMock = new Mock<IDateExtra>();
            dateMock.SetupGet(d => d.UtcNow)
                .Returns(setTime)
                .Verifiable("Now was never called.");

            var claimMock = new Mock<IRepository<ClaimTokenSchema>>();

            var result = new Task<ClaimTokenSchema>(() =>
            {
                return new ClaimTokenSchema
                {
                    IsExisting = true,
                    CreatedAt = new BsonDateTime(setTime),
                    Access = null,
                };
            });
            result.Start();

            claimMock.Setup(s => s.FindOne(It.IsAny<FilterDefinition<ClaimTokenSchema>>()))
                .Returns(result)
                .Verifiable("A claim token is not searched for.");

            var client = CustomHost.Create(claimMock: claimMock, dateMock: dateMock);

            var request = CustomRequest.Create(HttpMethod.Get, "/users/claims");
            request.Headers.TryAddWithoutValidation("Cookie", "ExperienceCapture-Claim-Token=" + "ok");
            var response = await client.SendAsync(request);

            var body = await response.Content.ReadAsStringAsync();

            Assert.True(
                response.StatusCode == HttpStatusCode.Accepted,
                "Getting a pending claim token is not accepted.");

            Assert.True(body == "PENDING", "Getting a pending claim does not have the correct message.");
        }

        [Fact]
        public async Task UpdateIsCalledGetClaims()
        {
            var setTime = new DateProvider().UtcNow;
            var dateMock = new Mock<IDateExtra>();
            dateMock.SetupGet(d => d.UtcNow)
                .Returns(setTime)
                .Verifiable("Now was never called.");

            var claimMock = new Mock<IRepository<ClaimTokenSchema>>();

            var result = new Task<ClaimTokenSchema>(() =>
            {
                return new ClaimTokenSchema
                {
                    IsExisting = true,
                    CreatedAt = new BsonDateTime(setTime),
                    Access = ObjectId.GenerateNewId(),
                };
            });
            result.Start();

            claimMock.Setup(s => s.FindOne(It.IsAny<FilterDefinition<ClaimTokenSchema>>()))
                .Returns(result)
                .Verifiable("A claim token is not searched for.");

            claimMock.Setup(s => s.Update(
                    It.IsAny<FilterDefinition<ClaimTokenSchema>>(),
                    It.IsAny<UpdateDefinition<ClaimTokenSchema>>()))
                .Verifiable("A claim token is not updated.");

            var client = CustomHost.Create(claimMock: claimMock, dateMock: dateMock);

            var request = CustomRequest.Create(HttpMethod.Get, "/users/claims");
            request.Headers.TryAddWithoutValidation("Cookie", "ExperienceCapture-Claim-Token=" + "ok");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ResponceIsValidGetClaims()
        {
            var setTime = new DateProvider().UtcNow;
            var dateMock = new Mock<IDateExtra>();
            dateMock.SetupGet(d => d.UtcNow)
                .Returns(setTime)
                .Verifiable("Now was never called.");

            var claimMock = new Mock<IRepository<ClaimTokenSchema>>();

            var result = new Task<ClaimTokenSchema>(() =>
            {
                return new ClaimTokenSchema
                {
                    IsExisting = true,
                    CreatedAt = new BsonDateTime(setTime),
                    Access = ObjectId.GenerateNewId(),
                    AccessToken = "example",
                };
            });
            result.Start();

            claimMock.Setup(s => s.FindOne(It.IsAny<FilterDefinition<ClaimTokenSchema>>()))
                .Returns(result)
                .Verifiable("A claim token is not searched for.");

            claimMock.Setup(s => s.Update(
                    It.IsAny<FilterDefinition<ClaimTokenSchema>>(),
                    It.IsAny<UpdateDefinition<ClaimTokenSchema>>()))
                .Verifiable("A claim token is not updated.");

            var client = CustomHost.Create(claimMock: claimMock, dateMock: dateMock);

            var request = CustomRequest.Create(HttpMethod.Get, "/users/claims");
            request.Headers.TryAddWithoutValidation("Cookie", "ExperienceCapture-Claim-Token=" + "ok");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var data = BsonSerializer.Deserialize<AccessTokenResponce>(body);

            Assert.False(data.AccessToken == null, "Access token data is null.");
            Assert.False(data.AccessToken == string.Empty, "Access token data is empty.");
            Assert.False(data.Expiration == null, "Access token expiration data is null.");
        }

        [Fact]
        public async Task ResponceIsValidBsonGetClaims()
        {
            var setTime = new DateProvider().UtcNow;
            var dateMock = new Mock<IDateExtra>();
            dateMock.SetupGet(d => d.UtcNow)
                .Returns(setTime)
                .Verifiable("Now was never called.");

            var claimMock = new Mock<IRepository<ClaimTokenSchema>>();

            var result = new Task<ClaimTokenSchema>(() =>
            {
                return new ClaimTokenSchema
                {
                    IsExisting = true,
                    CreatedAt = new BsonDateTime(setTime),
                    Access = ObjectId.GenerateNewId(),
                    AccessToken = "example",
                };
            });
            result.Start();

            claimMock.Setup(s => s.FindOne(It.IsAny<FilterDefinition<ClaimTokenSchema>>()))
                .Returns(result)
                .Verifiable("A claim token is not searched for.");

            claimMock.Setup(s => s.Update(
                    It.IsAny<FilterDefinition<ClaimTokenSchema>>(),
                    It.IsAny<UpdateDefinition<ClaimTokenSchema>>()))
                .Verifiable("A claim token is not updated.");

            var client = CustomHost.Create(claimMock: claimMock, dateMock: dateMock);

            var request = CustomRequest.Create(HttpMethod.Get, "/users/claims?bson=true");
            request.Headers.TryAddWithoutValidation("Cookie", "ExperienceCapture-Claim-Token=" + "ok");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsByteArrayAsync();
            var data = BsonSerializer.Deserialize<AccessTokenResponce>(body);

            Assert.False(data.AccessToken == null, "Access token data is null.");
            Assert.False(data.AccessToken == string.Empty, "Access token data is empty.");
            Assert.False(data.Expiration == null, "Access token expiration data is null.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("/")]
        [InlineData("?")]
        [InlineData("/?")]
        [InlineData("/?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task OtherMethodsClaims(string input)
        {
            var client = CustomHost.Create();

            var requestPut = CustomRequest.Create(HttpMethod.Put, $"/users/claims{input}");
            var responsePut = await client.SendAsync(requestPut);

            Assert.True(
                responsePut.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Putting claims is an allowed method.");

            var requestPatch = CustomRequest.Create(HttpMethod.Patch, $"/users/claims{input}");
            var responsePatch = await client.SendAsync(requestPatch);

            Assert.True(
                responsePatch.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Patching claims is an allowed method.");

            var requestDelete = CustomRequest.Create(HttpMethod.Delete, $"/users/claims{input}");
            var responseDelete = await client.SendAsync(requestDelete);

            Assert.True(
                responseDelete.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Deleting claims is an allowed method.");
        }

        [Theory]
        [InlineData("{}")]
        [InlineData("{ \"password\": 1234567890 }")]
        [InlineData("{ \"password\": \"\" }")]
        [InlineData("{ \"fail\": \"1234567890\" }")]
        public async Task BodyIsCheckedPostAdmin(string input)
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Post, "/users/signUp/admin");
            request.Content = new StringContent(input, Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Creating a new sign-up token from admin is not a bad request.");
        }

        [Fact]
        public async Task BadPasswordIsUnauthorizedPostAdmin()
        {
            var envMock = new Mock<IAppEnvironment>();
            envMock.SetupGet(e => e.PasswordHash)
                .Returns("CLi4XS7q4DNwiYSWI6JI7rqEz9KHvI27E3mm9i0Xr6Q=");

            var client = CustomHost.Create(envMock: envMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/users/signUp/admin");
            request.Content = new StringContent("{ \"password\": \"fail\" }", Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.Unauthorized,
                "Creating a new sign-up token from admin is not unauthorized.");
        }

        [Fact]
        public async Task UpdateIsCalledPostAdmin()
        {
            var envMock = new Mock<IAppEnvironment>();
            envMock.SetupGet(e => e.PasswordHash)
                .Returns("CLi4XS7q4DNwiYSWI6JI7rqEz9KHvI27E3mm9i0Xr6Q=");

            var signUpMock = new Mock<IRepository<SignUpTokenSchema>>();

            signUpMock.Setup(s => s.Add(It.IsAny<SignUpTokenSchema>()))
                .Verifiable("A sign-up token is never added");

            var client = CustomHost.Create(signUpMock: signUpMock, envMock: envMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/users/signUp/admin");
            request.Content = new StringContent("{ \"password\": \"7JO8e1TmORUCSvHAdcp2eBFAdwh02o+WwLiLjdF4Kkc=\" }", Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task SkipWhenNotTrueIsUnauthorizedPostAdmin()
        {
            var envMock = new Mock<IAppEnvironment>();
            envMock.SetupGet(e => e.PasswordHash)
                .Returns("CLi4XS7q4DNwiYSWI6JI7rqEz9KHvI27E3mm9i0Xr6Q=");

            envMock.SetupGet(e => e.SkipValidation)
                .Returns("false");

            var client = CustomHost.Create(envMock: envMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/users/signUp/admin");
            request.Content = new StringContent("{ \"password\": \"fail\" }", Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.Unauthorized,
                "Creating a new sign-up token from admin is allowed.");
        }

        [Fact]
        public async Task ResponceIsValidPostAdmin()
        {
            var envMock = new Mock<IAppEnvironment>();
            envMock.SetupGet(e => e.PasswordHash)
                .Returns("CLi4XS7q4DNwiYSWI6JI7rqEz9KHvI27E3mm9i0Xr6Q=");

            var signUpMock = new Mock<IRepository<SignUpTokenSchema>>();

            signUpMock.Setup(s => s.Add(It.IsAny<SignUpTokenSchema>()))
                .Verifiable("A sign-up token is never added");

            var client = CustomHost.Create(signUpMock: signUpMock, envMock: envMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/users/signUp/admin");
            request.Content = new StringContent("{ \"password\": \"7JO8e1TmORUCSvHAdcp2eBFAdwh02o+WwLiLjdF4Kkc=\" }", Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var data = BsonSerializer.Deserialize<SignUpTokenResponce>(body);

            Assert.False(data.SignUpToken == null, "An admin token data is null.");
            Assert.False(data.SignUpToken == string.Empty, "An admin token data is empty.");
            Assert.False(data.Expiration == null, "An admin expiration data is null.");
        }

        [Fact]
        public async Task ResponceIsValidBsonPostAdmin()
        {
            var envMock = new Mock<IAppEnvironment>();
            envMock.SetupGet(e => e.PasswordHash)
                .Returns("CLi4XS7q4DNwiYSWI6JI7rqEz9KHvI27E3mm9i0Xr6Q=");

            var signUpMock = new Mock<IRepository<SignUpTokenSchema>>();

            signUpMock.Setup(s => s.Add(It.IsAny<SignUpTokenSchema>()))
                .Verifiable("A sign-up token is never added");

            var client = CustomHost.Create(signUpMock: signUpMock, envMock: envMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/users/signUp/admin?bson=true");
            request.Content = new StringContent("{ \"password\": \"7JO8e1TmORUCSvHAdcp2eBFAdwh02o+WwLiLjdF4Kkc=\" }", Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsByteArrayAsync();
            var data = BsonSerializer.Deserialize<SignUpTokenResponce>(body);

            Assert.False(data.SignUpToken == null, "An admin token data is null.");
            Assert.False(data.SignUpToken == string.Empty, "An admin token data is empty.");
            Assert.False(data.Expiration == null, "An admin expiration data is null.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("/")]
        [InlineData("?")]
        [InlineData("/?")]
        [InlineData("/?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task OtherMethodsAdmin(string input)
        {
            var client = CustomHost.Create();

            var requestPut = CustomRequest.Create(HttpMethod.Put, $"/users/signUp/admin{input}");
            var responsePut = await client.SendAsync(requestPut);

            Assert.True(
                responsePut.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Putting admin is an allowed method.");

            var requestPatch = CustomRequest.Create(HttpMethod.Patch, $"/users/signUp/admin{input}");
            var responsePatch = await client.SendAsync(requestPatch);

            Assert.True(
                responsePatch.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Patching admin is an allowed method.");

            var requestDelete = CustomRequest.Create(HttpMethod.Delete, $"/users/signUp/admin{input}");
            var responseDelete = await client.SendAsync(requestDelete);

            Assert.True(
                responseDelete.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Deleting admin is an allowed method.");

            var requestGet = CustomRequest.Create(HttpMethod.Get, $"/users/signUp/admin{input}");
            var responseGet = await client.SendAsync(requestGet);

            Assert.True(
                responseGet.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Getting admin is an allowed method.");
        }
    }
}