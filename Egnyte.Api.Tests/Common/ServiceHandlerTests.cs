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
        private const string RetryAfter = "20";
        private const string Alloted = "100";
        private const string Current = "101";

        [Test]
        public async Task SendRequestAsync_ThrowsQPSLimitExceededException_WhenAccountOverQPSLimit()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);
            const string Content = "<h1>Developer Over Qps</h1>";
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Forbidden,
                Content = new StringContent(Content)
            };
            responseMessage.Headers.Add("retry-after", RetryAfter);
            responseMessage.Headers.Add("x-mashery-error-code", "ERR_403_DEVELOPER_OVER_QPS");
            responseMessage.Headers.Add("x-accesstoken-qps-allotted", Alloted);
            responseMessage.Headers.Add("x-accesstoken-qps-current", Current);
            
            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                    Task.FromResult(responseMessage);

            var serviceHandler = new ServiceHandler<string>(httpClient);

            var exception = await AssertExtensions.ThrowsAsync<QPSLimitExceededException>(
                () => serviceHandler.SendRequestAsync(new HttpRequestMessage(HttpMethod.Get, "https://dev.egnyte.com/api")));
            
            Assert.IsNull(exception.InnerException);
            Assert.AreEqual("Account over QPS limit", exception.Message);

            Assert.AreEqual(RetryAfter, exception.RetryAfter);
            Assert.AreEqual(Current, exception.Current);
            Assert.AreEqual(Alloted, exception.Allotted);
        }

        [Test]
        public async Task SendRequestAsync_ThrowsRateLimitExceededException_WhenAccountOverRateLimit()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);
            const string Content = "<h1>Developer Over Qps</h1>";
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Forbidden,
                Content = new StringContent(Content)
            };
            responseMessage.Headers.Add("retry-after", RetryAfter);
            responseMessage.Headers.Add("x-mashery-error-code", "ERR_403_DEVELOPER_OVER_RATE");
            responseMessage.Headers.Add("x-accesstoken-quota-allotted", Alloted);
            responseMessage.Headers.Add("x-accesstoken-quota-current", Current);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                    Task.FromResult(responseMessage);

            var serviceHandler = new ServiceHandler<string>(httpClient);

            var exception = await AssertExtensions.ThrowsAsync<RateLimitExceededException>(
                () => serviceHandler.SendRequestAsync(new HttpRequestMessage(HttpMethod.Get, "https://dev.egnyte.com/api")));

            Assert.IsNull(exception.InnerException);
            Assert.AreEqual("Account over rate limit", exception.Message);

            Assert.AreEqual(RetryAfter, exception.RetryAfter);
            Assert.AreEqual(Current, exception.Current);
            Assert.AreEqual(Alloted, exception.Allotted);
        }
    }
}
