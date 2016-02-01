using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Egnyte.Api.Tests.Users
{
    [TestFixture]
    public class DeleteLinkTests
    {
        [Test]
        public async Task DeleteLink_ReturnsSuccess()
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
            var isSucccess = await egnyteClient.Links.DeleteLink("link_1234");

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            var content = httpHandlerMock.GetRequestContentAsString();
            Assert.IsTrue(isSucccess);
            Assert.AreEqual("https://acme.egnyte.com/pubapi/v1/links/link_1234", requestMessage.RequestUri.ToString());
            Assert.IsNull(content);
        }

        [Test]
        public async Task DeleteLink_WhenIdIsWrong_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Links.DeleteLink("   "));

            Assert.IsTrue(exception.Message.Contains("linkId"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
