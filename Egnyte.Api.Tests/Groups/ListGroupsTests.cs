using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Egnyte.Api.Tests.Groups
{
    [TestFixture]
    public class ListGroupsTests
    {
        const string ListGroupsResponseContent = @"
            {
                ""schemas"": [""urn:scim:schemas:core:1.0""],
                ""totalResults"":2,
                ""itemsPerPage"":10,
                ""startIndex"":5,
                ""resources"":
                [
	                {
	                    ""id"":""1c859a1c-8650-4003-b78e-20aeaca7ac0c"",
	                    ""displayName"":""Users 2""
	                },
	                {
	                    ""id"":""21452dfa-ec5f-416f-8816-63bb2d85dfab"",
	                    ""displayName"":""Users""
	                }
                ]
            }";

        [Test]
        public async Task ListGroups_ReturnsSuccess()
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
                                ListGroupsResponseContent,
                                Encoding.UTF8,
                                "application/json")
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var userList = await egnyteClient.Groups.ListGroups(5, 10);

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v2/groups?startIndex=5&count=10",
                requestMessage.RequestUri.ToString());

            Assert.AreEqual(2, userList.TotalResults);
            Assert.AreEqual(10, userList.ItemsPerPage);
            Assert.AreEqual(5, userList.StartIndex);
            Assert.AreEqual(2, userList.Resources.Count);

            var firstGroup = userList.Resources.FirstOrDefault(
                u => u.Id == "1c859a1c-8650-4003-b78e-20aeaca7ac0c");
            Assert.IsNotNull(firstGroup);
            Assert.AreEqual("Users 2", firstGroup.DisplayName);

            var secondGroup = userList.Resources.FirstOrDefault(
                u => u.Id == "21452dfa-ec5f-416f-8816-63bb2d85dfab");
            Assert.IsNotNull(secondGroup);
            Assert.AreEqual("Users", secondGroup.DisplayName);
        }
        
        [Test]
        public async Task ListGroups_WhenFilterIsSpecified_ReturnsSuccess()
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
                                ListGroupsResponseContent,
                                Encoding.UTF8,
                                "application/json")
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var userList = await egnyteClient.Groups.ListGroups(1, 10, @"displayname co ""ccou""");

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v2/groups?startIndex=1&count=10&filter=displayname co \"ccou\"",
                requestMessage.RequestUri.ToString());

            Assert.AreEqual(2, userList.TotalResults);
            Assert.AreEqual(10, userList.ItemsPerPage);
            Assert.AreEqual(5, userList.StartIndex);
            Assert.AreEqual(2, userList.Resources.Count);
        }

        [Test]
        public async Task ListGroups_WhenStartIndexEqualsToZero_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentOutOfRangeException>(
                            () => egnyteClient.Groups.ListGroups(0));

            Assert.IsTrue(exception.Message.Contains("startIndex"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task ListGroups_WhenCountIsLessThanZero_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentOutOfRangeException>(
                            () => egnyteClient.Groups.ListGroups(count: -1));

            Assert.IsTrue(exception.Message.Contains("count"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task ListGroups_WhenCountIsTooBig_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentOutOfRangeException>(
                            () => egnyteClient.Groups.ListGroups(count: 115));

            Assert.IsTrue(exception.Message.Contains("count"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
