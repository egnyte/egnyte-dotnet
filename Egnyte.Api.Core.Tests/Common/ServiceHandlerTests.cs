using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Egnyte.Api.Common;
using NUnit.Framework;

namespace Egnyte.Api.Tests.Common
{
    [TestFixture]
    public class ServiceHandlerTests
    {
        [Test]
        public async Task SendRequestAsync_ThrowsException_WhenAccountOverRateLimit()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);
            const string Content = "<h1>Developer Over Qps</h1>";
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Forbidden,
                Content = new StringContent(Content)
            };
            responseMessage.Headers.Add("Retry-After", "70050");
            
            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                    Task.FromResult(responseMessage);

            var serviceHandler = new ServiceHandler<string>(httpClient);

            var exception = await AssertExtensions.ThrowsAsync<EgnyteApiException>(
                () => serviceHandler.SendRequestAsync(new HttpRequestMessage(HttpMethod.Get, "https://dev.egnyte.com/api")));

            Assert.AreEqual(HttpStatusCode.Forbidden, exception.StatusCode);
            Assert.IsNull(exception.InnerException);
            Assert.AreEqual(Content, exception.Message);

            Assert.True(exception.Headers.Any());
            Assert.AreEqual("70050", exception.Headers["Retry-After"]);
        }
    }
}
