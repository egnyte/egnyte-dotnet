using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Egnyte.Api.Tests.ProjectFolders
{
    [TestFixture]
    public class MarkFolderAsProjectTests
    {
        [Test]
        public async Task MarkFolderAsProject_ReturnsSuccess()
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
            var markFolderAsProjectResponse = await egnyteClient.ProjectFolders.MarkFolderAsProject(
                rootFolderId: "ABC-123",
                name: "Acme Widgets HQ",
                status: "pending",
                description: "Redesigned HQ for Acme Widgets",
                startDate: new DateTime(2022, 11, 20),
                completionDate: new DateTime(2022, 11, 21)
            );

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v1/project-folders",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual(HttpMethod.Post, requestMessage.Method);
            Assert.IsTrue(markFolderAsProjectResponse);
        }

        [Test]
        public async Task MarkFolderAsProject_WhenRootFolderIdIsEmpty_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.ProjectFolders.MarkFolderAsProject(
                    rootFolderId: string.Empty,
                    name: "Acme Widgets HQ",
                    status: "pending"));

            Assert.IsTrue(exception.Message.Contains("rootFolderId"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task MarkFolderAsProject_WhenNameIsEmpty_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.ProjectFolders.MarkFolderAsProject(
                    rootFolderId: "ABC-123",
                    name: string.Empty,
                    status: "pending"));

            Assert.IsTrue(exception.Message.Contains("name"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task MarkFolderAsProject_WhenStatusIsEmpty_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.ProjectFolders.MarkFolderAsProject(
                    rootFolderId: "ABC-123",
                    name: "Acme Widgets HQ",
                    status: string.Empty));

            Assert.IsTrue(exception.Message.Contains("status"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
