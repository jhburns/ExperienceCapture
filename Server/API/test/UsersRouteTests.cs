namespace Carter.Tests.Route.NewSignUp
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

    public class UsersTests
    {
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