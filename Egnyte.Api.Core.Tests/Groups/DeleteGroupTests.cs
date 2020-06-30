using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Egnyte.Api.Tests.Groups
{
    [TestFixture]
    public class DeleteGroupTests
    {
        [Test]
        public async Task DeleteGroup_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(string.Empty)
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var isSucccess = await egnyteClient.Groups.DeleteGroup("myGroup");

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            var content = httpHandlerMock.GetRequestContentAsString();

            Assert.IsTrue(isSucccess);
            Assert.AreEqual(HttpMethod.Delete, requestMessage.Method);
            Assert.AreEqual("https://acme.egnyte.com/pubapi/v2/groups/myGroup", requestMessage.RequestUri.ToString());
            Assert.IsNull(content);
        }

        [Test]
        public async Task DeleteGroup_WhenGroupIdIsEmpty_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Groups.DeleteGroup(string.Empty));

            Assert.IsTrue(exception.Message.Contains("groupId"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
