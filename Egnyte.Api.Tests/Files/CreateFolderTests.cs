using System.Threading.Tasks;

namespace Egnyte.Api.Tests.Files
{
    using System;
    using System.Net;
    using System.Net.Http;

    using NUnit.Framework;

    [TestFixture]
    public class CreateFolderTests
    {
        [Test]
        public async Task CreateFolder_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.Created,
                            Content = new StringContent(string.Empty)
                        });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var isSucccess = await egnyteClient.Files.CreateFolder("path");

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            var content = httpHandlerMock.GetRequestContentAsString();
            Assert.IsTrue(isSucccess);
            Assert.AreEqual("https://acme.egnyte.com/pubapi/v1/fs/path", requestMessage.RequestUri.ToString());
            Assert.AreEqual("{\"action\": \"add_folder\"}", content);
        }

        [Test]
        public async Task CreateFolder_WhenLanguageSpecificCharactersAreInUrl_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.Created,
                        Content = new StringContent(string.Empty)
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var isSucccess = await egnyteClient.Files.CreateFolder("[T]{F} ąśćęół");

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.IsTrue(isSucccess);
            Assert.AreEqual(
                "/pubapi/v1/fs/%5BT%5D%7BF%7D%20%C4%85%C5%9B%C4%87%C4%99%C3%B3%C5%82",
                requestMessage.RequestUri.AbsolutePath);
        }

        [Test]
        public async Task CreateFolder_WhenNoPathSpecified_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Files.CreateFolder(string.Empty));

            Assert.IsTrue(exception.Message.Contains("path"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
