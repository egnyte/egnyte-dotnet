using Egnyte.Api.Users;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Egnyte.Api.Tests.Users
{
    public class GetUserListTests
    {
        const string GetUserListResponseContent = @"
            {
                ""totalResults"": 100,
                ""itemsPerPage"": 2,
                ""startIndex"": 6,
                ""Resources"": [
                    {
                        ""id"": ""12345678"",
                        ""userName"": ""jmiller"",
                        ""externalId"": ""S-1-5-21-3623811015-3361044348-30300820-1013"",
                        ""email"": ""john.miller@example.com"",
                        ""name"": {
                            ""familyName"": ""Miller"",
                            ""givenName"": ""John""
                        },
                        ""active"": ""true"",
                        ""locked"": ""false"",
                        ""authType"": ""ad"",
                        ""userType"": ""admin"",
                        ""userPrincipalName"": ""jmiller@example.com""
                    },
                    {
                        ""id"": ""17654328"",
                        ""userName"": ""bjensen"",
                        ""externalId"": ""S-1-5-21-3623811015-3361044348-30300820-1014"",
                        ""email"": ""bjensen@example.com"",
                        ""name"": {
                            ""familyName"": ""Jensen"",
                            ""givenName"": ""Barbara""
                        },
                        ""active"": ""true"",
                        ""locked"": ""false"",
                        ""authType"": ""sso"",
                        ""userType"": ""power"",
                        ""idpUserId"": ""bjensen""
                    }
                ]
            }";

        [Test]
        public async Task GetUserList_ReturnsSuccess()
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
                                GetUserListResponseContent,
                                Encoding.UTF8,
                                "application/json")
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var userList = await egnyteClient.Users.GetUserList(6, 2);

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v2/users?startIndex=6&count=2",
                requestMessage.RequestUri.ToString());

            Assert.AreEqual(100, userList.TotalResults);
            Assert.AreEqual(2, userList.ItemsPerPage);
            Assert.AreEqual(6, userList.StartIndex);
            Assert.AreEqual(2, userList.Users.Count);

            var firstUser = userList.Users.FirstOrDefault(u => u.Id == 12345678);
            Assert.IsNotNull(firstUser);
            Assert.AreEqual("jmiller", firstUser.UserName);
            Assert.AreEqual("S-1-5-21-3623811015-3361044348-30300820-1013", firstUser.ExternalId);
            Assert.AreEqual("john.miller@example.com", firstUser.Email);
            Assert.AreEqual("Miller", firstUser.FamilyName);
            Assert.AreEqual("John", firstUser.GivenName);
            Assert.IsTrue(firstUser.Active);
            Assert.IsFalse(firstUser.Locked);
            Assert.AreEqual(UserAuthType.AD, firstUser.AuthType);
            Assert.AreEqual(UserType.Administrator, firstUser.UserType);
            Assert.AreEqual("jmiller@example.com", firstUser.UserPrincipalName);
        }
        
        [Test]
        public async Task GetUserList_WhenFilterIsSpecified_ReturnsSuccess()
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
                                GetUserListResponseContent,
                                Encoding.UTF8,
                                "application/json")
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var userList = await egnyteClient.Users.GetUserList(1, 10, "email eq \"john@egnyte.com\"");

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v2/users?startIndex=1&count=10&filter=email eq \"john@egnyte.com\"",
                requestMessage.RequestUri.ToString());

            Assert.AreEqual(100, userList.TotalResults);
            Assert.AreEqual(2, userList.ItemsPerPage);
            Assert.AreEqual(6, userList.StartIndex);
            Assert.AreEqual(2, userList.Users.Count);
        }

        [Test]
        public async Task GetUserList_WhenStartIndexEqualsToZero_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentOutOfRangeException>(
                            () => egnyteClient.Users.GetUserList(0));

            Assert.IsTrue(exception.Message.Contains("startIndex"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task GetUserList_WhenCountIsLessThanZero_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentOutOfRangeException>(
                            () => egnyteClient.Users.GetUserList(count: -1));

            Assert.IsTrue(exception.Message.Contains("count"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task GetUserList_WhenCountIsTooBig_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentOutOfRangeException>(
                            () => egnyteClient.Users.GetUserList(count: 115));

            Assert.IsTrue(exception.Message.Contains("count"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
