using NUnit.Framework;
using System;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Egnyte.Api.ProjectFolders;

namespace Egnyte.Api.Tests.ProjectFolders
{
    public class UpdateProjectTests
    {
        [Test]
        public async Task UpdateProject_ReturnsSuccess()
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
            var updateProjectResponse = await egnyteClient.ProjectFolders.UpdateProject(
                name: "Acme Widgets HQ",
                id: "P123",
                status: "pending",
                projectId: "ABC123",
                customerName: "Acme Widgets",
                startDate: new DateTime(2022, 11, 20),
                location: new ProjectFolderLocation
                {
                    StreetAddress1 = "123 Main St",
                    City = "Anytown",
                    State = "CA",
                    PostalCode = "99999",
                    Country = "USA"
                }
                );

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v2/project-folders/P123",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual(HttpMethod.Patch, requestMessage.Method);
            Assert.IsTrue(updateProjectResponse);
        }

        [Test]
        public async Task UpdateProject_WhenNameIsEmpty_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.ProjectFolders.UpdateProject(
                    name: string.Empty,
                    id: "P123",
                    status: "pending",
                    projectId: "ABC123"));

            Assert.IsTrue(exception.Message.Contains("name"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task UpdateProject_WhenStatusIsEmpty_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.ProjectFolders.UpdateProject(
                    name: "Acme Widgets HQ",
                    id: "P123",
                    status: string.Empty,
                    projectId: "ABC123"));

            Assert.IsTrue(exception.Message.Contains("status"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task UpdateProject_WhenProjectIdIsEmpty_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.ProjectFolders.UpdateProject(
                    name: "Acme Widgets HQ",
                    id: "P123",
                    status: "pending",
                    projectId: string.Empty));

            Assert.IsTrue(exception.Message.Contains("projectId"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
