using System.Threading.Tasks;

namespace Egnyte.Api.Tests.Files
{
    using System.Net;
    using System.Net.Http;

    using NUnit.Framework;
    using System.Text;
    using System;
    [TestFixture]
    public class UpdateUserTests
    {
        const string UpdateUserResponse = @"
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

        const string UpdateUserRequestContent = @"
            {
                ""email"" : ""mik@egnyte.com"",
                ""familyName"" : ""Caine"",
                ""givenName"" : ""Michael"",
                ""active"" : ""true"",
                ""sendInvite"" : ""true"",
                ""authType"" : ""egnyte"",
                ""userType"" : ""power""
            }";

        [Test]
        public async Task UpdateUser_ReturnsSuccess()
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
                                UpdateUserResponse,
                                Encoding.UTF8,
                                "application/json")
                    });

            var userToUpdate = UserToUpdate();
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var existingUser = await egnyteClient.Users.UpdateUser(userToUpdate);

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v2/users/" + userToUpdate.Id,
                requestMessage.RequestUri.ToString());
            Assert.AreEqual(userToUpdate.Active, existingUser.Active);
            Assert.AreEqual(userToUpdate.AuthType, existingUser.AuthType);
            Assert.AreEqual(userToUpdate.Email, existingUser.Email);
            Assert.AreEqual(userToUpdate.FamilyName, existingUser.FamilyName);
            Assert.AreEqual(userToUpdate.GivenName, existingUser.GivenName);
            Assert.AreEqual(userToUpdate.UserType, existingUser.UserType);

            var content = httpHandlerMock.GetRequestContentAsString();
            Assert.AreEqual(
                RemoveWhitespaces(UpdateUserRequestContent),
                RemoveWhitespaces(content));
        }

        [Test]
        public async Task CreateUser_WithoutId_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var userToUpdate = UserToUpdate();
            userToUpdate.Id = 0;
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentOutOfRangeException>(
                            () => egnyteClient.Users.UpdateUser(userToUpdate));

            Assert.IsTrue(exception.Message.Contains("Id"));
            Assert.IsNull(exception.InnerException);
        }
        
        Users.UserUpdate UserToUpdate()
        {
            return new Users.UserUpdate
            {
                Id = 12345,
                Email = "mik@egnyte.com",
                FamilyName = "Caine",
                GivenName = "Michael",
                Active = true,
                SendInvite = true,
                AuthType = Users.UserAuthType.Internal_Egnyte,
                UserType = Users.UserType.PowerUser
            };
        }

        string RemoveWhitespaces(string text)
        {
            return text.Replace(" ", "")
                .Replace("\n", "")
                .Replace("\t", "")
                .Replace("\r", "");
        }
    }
}
