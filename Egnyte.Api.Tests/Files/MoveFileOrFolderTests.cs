namespace Egnyte.Api.Tests.Files
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using NUnit.Framework;

    [TestFixture]
    public class MoveFileOrFolderTests
    {
        [Test]
        public async void MoveFile_ReturnsSuccess()
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
            var isSucccess = await egnyteClient.Files.MoveFileOrFolder("pathFrom", "pathTo");

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.IsTrue(isSucccess);
            Assert.AreEqual("https://acme.egnyte.com/pubapi/v1/fs/pathFrom", requestMessage.RequestUri.ToString());
        }

        [Test]
        public async void MoveFile_WhenNoPathSpecified_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Files.MoveFileOrFolder(string.Empty, "destination"));

            Assert.IsTrue(exception.Message.Contains("Parameter name: path"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async void MoveFile_WhenNoDestinationSpecified_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Files.MoveFileOrFolder("path", string.Empty));

            Assert.IsTrue(exception.Message.Contains("Parameter name: destination"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
