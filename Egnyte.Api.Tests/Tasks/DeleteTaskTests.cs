using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Egnyte.Api.Tests.Tasks
{
    [TestFixture]
    public class DeleteTaskTests
    {
        [Test]
        public async Task DeleteTask_ReturnsSuccess()
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

            var taskId = Guid.NewGuid().ToString();

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var isSucccess = await egnyteClient.Tasks.DeleteTask(taskId);

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            var content = httpHandlerMock.GetRequestContentAsString();
            Assert.IsTrue(isSucccess);
            Assert.AreEqual("https://acme.egnyte.com/pubapi/v1/tasks/" + taskId, requestMessage.RequestUri.ToString());
            Assert.IsNull(content);
        }

        [Test]
        public async Task DeleteTask_WhenIdIsWrong_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Tasks.DeleteTask("   "));

            Assert.IsTrue(exception.Message.Contains("taskId"));
            Assert.IsNull(exception.InnerException);
        }
    }
}