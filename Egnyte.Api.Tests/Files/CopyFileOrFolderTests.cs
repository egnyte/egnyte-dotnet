namespace Egnyte.Api.Tests.Files
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using NUnit.Framework;

    [TestFixture]
    public class CopyFileOrFolderTests
    {
        [Test]
        public async Task CopyFile_ReturnsSuccess()
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
            var isSucccess = await egnyteClient.Files.CopyFileOrFolder("pathFrom", "pathTo");

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            var content = httpHandlerMock.GetRequestContentAsString();
            Assert.IsTrue(isSucccess);
            Assert.AreEqual("https://acme.egnyte.com/pubapi/v1/fs/pathFrom", requestMessage.RequestUri.ToString());
            Assert.AreEqual("{\"action\": \"copy\", \"destination\": \"/pathTo\"}", content);
        }

        [Test]
        public async Task CopyFile_WhenNoPathSpecified_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Files.MoveFileOrFolder(string.Empty, "destination"));

            Assert.IsTrue(exception.Message.Contains("path"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task CopyFile_WhenNoDestinationSpecified_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Files.MoveFileOrFolder("path", string.Empty));

            Assert.IsTrue(exception.Message.Contains("destination"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
