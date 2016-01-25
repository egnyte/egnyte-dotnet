using System.Threading.Tasks;

namespace Egnyte.Api.Tests.Files
{
    using System.Net;
    using System.Net.Http;

    using NUnit.Framework;
    using System.Text;
    using System;
    using Egnyte.Api.Users;

    [TestFixture]
    public class GetUserTests
    {
        const string GetUserResponse = @"
            {
                ""id"":12824215693,
                ""userName"":""mik"",
                ""externalId"":""84079f93-f174-4c76-8522-3f30210fc14e"",
                ""email"":""mik@egnyte.com"",
                ""name"":
                {
                    ""formatted"":""Michael Caine"",
                    ""familyName"":""Caine"",
                    ""givenName"":""Michael""
                },
                ""active"":true,
                ""locked"":false,
                ""emailChangePending"":null,
                ""authType"":""egnyte"",
                ""userType"":""power"",
                ""role"":""Reviewer"",
                ""idpUserId"":""mik"",
                ""userPrincipalName"":""mik@egnyte.com"",
                ""expiryDate"":null,
                ""deleteOnExpiry"":null
            }";

        [Test]
        public async Task GetUser_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(
                                GetUserResponse,
                                Encoding.UTF8,
                                "application/json")
                    });
            
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var existingUser = await egnyteClient.Users.GetUser(12824215693);

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v2/users/12824215693",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual(12824215693, existingUser.Id);
            Assert.AreEqual("mik", existingUser.UserName);
            Assert.AreEqual("84079f93-f174-4c76-8522-3f30210fc14e", existingUser.ExternalId);
            Assert.AreEqual("mik@egnyte.com", existingUser.Email);
            Assert.AreEqual("Caine", existingUser.FamilyName);
            Assert.AreEqual("Michael", existingUser.GivenName);
            Assert.AreEqual(true, existingUser.Active);
            Assert.AreEqual(false, existingUser.Locked);
            Assert.AreEqual(UserAuthType.Internal_Egnyte, existingUser.AuthType);
            Assert.AreEqual(UserType.PowerUser, existingUser.UserType);
            Assert.AreEqual("mik", existingUser.IdpUserId);
            Assert.AreEqual("mik@egnyte.com", existingUser.UserPrincipalName);
        }

        [Test]
        public async Task GetUser_WithWrongId_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentOutOfRangeException>(
                            () => egnyteClient.Users.GetUser(0));

            Assert.IsTrue(exception.Message.Contains("id"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
