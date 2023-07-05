using NUnit.Framework;
using System;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;

namespace Egnyte.Api.Tests.ProjectFolders
{
    [TestFixture]
    public class RemoveProjectTests
    {
        [Test]
        public async Task RemoveProject_ReturnsSuccess()
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
            var removeProjectResponse = await egnyteClient.ProjectFolders.RemoveProject(projectId: "ABC-123");

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v2/project-folders/ABC-123",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual(HttpMethod.Delete, requestMessage.Method);
            Assert.IsTrue(removeProjectResponse);
        }

        [Test]
        public async Task RemoveProject_WhenProjectIdIsEmpty_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.ProjectFolders.RemoveProject(projectId: string.Empty));

            Assert.IsTrue(exception.Message.Contains("projectId"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
