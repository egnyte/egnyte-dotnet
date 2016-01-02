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
        public async void CreateFolder_WhenCouldNotConnect_ThrowsException()
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

            Assert.IsTrue(isSucccess);
        }

        [Test]
        public async void CreateFolder_WhenNoPathSpecified_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Files.CreateFolder(string.Empty));

            Assert.IsTrue(exception.Message.Contains("Parameter name: path"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
