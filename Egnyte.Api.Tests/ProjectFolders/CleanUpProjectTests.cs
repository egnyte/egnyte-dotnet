using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Egnyte.Api.Tests.ProjectFolders
{
    [TestFixture]
    public class CleanUpProjectTests
    {
        [Test]
        public async Task CleanUpProject_ReturnsSuccess()
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
            var cleanupProjectResponse = await egnyteClient.ProjectFolders.CleanUpProject(projectId: "ABC123", true);

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v1/project-folders/ABC123/cleanup",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual(HttpMethod.Post, requestMessage.Method);
            Assert.IsTrue(cleanupProjectResponse);
        }

        [Test]
        public async Task CleanUpProject_WhenProjectIdIsEmpty_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.ProjectFolders.CleanUpProject(projectId: string.Empty, true));

            Assert.IsTrue(exception.Message.Contains("projectId"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
