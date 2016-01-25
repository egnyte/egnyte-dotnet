using System.Threading.Tasks;

namespace Egnyte.Api.Tests.Files
{
    using System.Net;
    using System.Net.Http;

    using NUnit.Framework;
    using System.Text;
    using System;
    using Api.Users;
    [TestFixture]
    public class CreateUserTests
    {
        const string CreateUserResponse = @"
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

        const string CreateUserRequestContent = @"
            {
                ""userName"" : ""mik"",
                ""externalId"" : ""84079f93-f174-4c76-8522-3f30210fc14e"",
                ""email"" : ""mik@egnyte.com"",
                ""name"" :
                {
                    ""familyName"" : ""Caine"",
                    ""givenName"" : ""Michael""
                },
                ""active"" : ""true"",
                ""sendInvite"" : ""true"",
                ""authType"" : ""egnyte"",
                ""userType"" : ""power"",
                ""role"" : ""Reviewer""
            }";

        [Test]
        public async Task CreateUser_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.Created,
                            Content = new StringContent(
                                CreateUserResponse,
                                Encoding.UTF8,
                                "application/json")
                        });

            var newUser = GetNewUser();
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var existingUser = await egnyteClient.Users.CreateUser(newUser);

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual("https://acme.egnyte.com/pubapi/v2/users", requestMessage.RequestUri.ToString());
            Assert.AreEqual(newUser.Active, existingUser.Active);
            Assert.AreEqual(newUser.AuthType, existingUser.AuthType);
            Assert.AreEqual(newUser.Email, existingUser.Email);
            Assert.AreEqual(newUser.ExternalId, existingUser.ExternalId);
            Assert.AreEqual(newUser.FamilyName, existingUser.FamilyName);
            Assert.AreEqual(newUser.GivenName, existingUser.GivenName);
            Assert.AreEqual(newUser.Role, existingUser.Role);
            Assert.AreEqual(newUser.UserName, existingUser.UserName);
            Assert.AreEqual(newUser.UserType, existingUser.UserType);

            var content = httpHandlerMock.GetRequestContentAsString();
            Assert.AreEqual(
                RemoveWhitespaces(CreateUserRequestContent),
                RemoveWhitespaces(content));
        }

        [Test]
        public async Task CreateUser_WithoutUserName_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var newUser = GetNewUser();
            newUser.UserName = null;
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                            () => egnyteClient.Users.CreateUser(newUser));

            Assert.IsTrue(exception.Message.Contains("UserName"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task CreateUser_WithoutExternalId_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var newUser = GetNewUser();
            newUser.ExternalId = null;
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                            () => egnyteClient.Users.CreateUser(newUser));

            Assert.IsTrue(exception.Message.Contains("ExternalId"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task CreateUser_WithoutEmail_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var newUser = GetNewUser();
            newUser.Email = null;
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                            () => egnyteClient.Users.CreateUser(newUser));

            Assert.IsTrue(exception.Message.Contains("Email"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task CreateUser_WithoutFamilyName_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var newUser = GetNewUser();
            newUser.FamilyName = null;
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                            () => egnyteClient.Users.CreateUser(newUser));

            Assert.IsTrue(exception.Message.Contains("FamilyName"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task CreateUser_WithoutGivenName_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var newUser = GetNewUser();
            newUser.GivenName = null;
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                            () => egnyteClient.Users.CreateUser(newUser));

            Assert.IsTrue(exception.Message.Contains("GivenName"));
            Assert.IsNull(exception.InnerException);
        }

        NewUser GetNewUser()
        {
            return new NewUser
            {
                UserName = "mik",
                ExternalId = "84079f93-f174-4c76-8522-3f30210fc14e",
                Email = "mik@egnyte.com",
                FamilyName = "Caine",
                GivenName = "Michael",
                Active = true,
                SendInvite = true,
                AuthType = UserAuthType.Internal_Egnyte,
                UserType = UserType.PowerUser,
                Role = "Reviewer"
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
