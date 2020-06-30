using System.Threading.Tasks;

namespace Egnyte.Api.Tests.Files
{
    using System;
    using System.Net;
    using System.Net.Http;

    using NUnit.Framework;

    [TestFixture]
    public class DeleteFileOrFolderTests
    {
        [Test]
        public async Task DeleteFolder_ReturnsSuccess()
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
            var isSucccess = await egnyteClient.Files.DeleteFileOrFolder("path");

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            var content = httpHandlerMock.GetRequestContentAsString();
            Assert.IsTrue(isSucccess);
            Assert.AreEqual("https://acme.egnyte.com/pubapi/v1/fs/path", requestMessage.RequestUri.ToString());
            Assert.IsNull(content);
        }

        [Test]
        public async Task DeleteFolder_WithEntryId_ReturnsSuccess()
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

            var entryId = "9355165a-e599-4148-88c5-0d3552493e2f";
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var isSucccess = await egnyteClient.Files.DeleteFileOrFolder("path", entryId);

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            var content = httpHandlerMock.GetRequestContentAsString();
            Assert.IsTrue(isSucccess);
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v1/fs/path?entry_id=" + entryId,
                requestMessage.RequestUri.ToString());
            Assert.IsNull(content);
        }

        [Test]
        public async Task DeleteFolder_WhenNoPathSpecified_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Files.DeleteFileOrFolder(string.Empty));

            Assert.IsTrue(exception.Message.Contains("path"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
