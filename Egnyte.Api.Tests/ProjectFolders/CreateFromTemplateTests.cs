using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Egnyte.Api.ProjectFolders;

namespace Egnyte.Api.Tests.ProjectFolders
{
    [TestFixture]
    public class CreateFromTemplateTests
    {
        const string CreateProjectFolderFromTemplateResponseContent = @"
           {
                ""groupsCreated"": [
                  {
                    ""id"" : ""8b149a9e-cafc-4c86-bc0c-a6fbacd5e16f"",
                    ""name"" : ""ABC123 - Project Team""
                  },
                  {
                    ""id"" : ""a183d648-dce7-4aa6-a946-993890c5a4d8"",
                    ""name"" : ""ABC123 - Project Managers""
                  },
                  {
                    ""id"" : ""f42c20f1-1205-4083-b8db-04df9c8706bf"",
                    ""name"" : ""ABC123 - BIM""
                  },
                  {
                    ""id"" : ""c988438b-e5f0-4ed9-b788-0f4265115926"",
                    ""name"" : ""ABC123 - External""
                  }
                ]
            } ";

        [Test]
        public async Task CreateProjectFromTemplate_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                    Task.FromResult(
                        new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = new StringContent(CreateProjectFolderFromTemplateResponseContent)
                        });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var createFromTemplateResponse = await egnyteClient.ProjectFolders.CreateProjectFromTemplate(
                parentFolderId: "7c30e681-bf0d-462e-b6cf-7471844962df",
                templateFolderId: "c881b13e-3fac-4a88-88ea-7221ef2855d8",
                folderName: "ABC123 - 123 Main St",
                name: "Acme Widgets HQ",
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
                "https://acme.egnyte.com/pubapi/v2/project-folders",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual(HttpMethod.Post, requestMessage.Method);

            Assert.AreEqual(4, createFromTemplateResponse.GroupsCreated.Length);
            Assert.AreEqual(1, createFromTemplateResponse.GroupsCreated.Count(u =>
                u.Id == "8b149a9e-cafc-4c86-bc0c-a6fbacd5e16f" && u.Name == "ABC123 - Project Team"));
            Assert.AreEqual(1, createFromTemplateResponse.GroupsCreated.Count(u =>
                u.Id == "a183d648-dce7-4aa6-a946-993890c5a4d8" && u.Name == "ABC123 - Project Managers"));
            Assert.AreEqual(1, createFromTemplateResponse.GroupsCreated.Count(u =>
                u.Id == "f42c20f1-1205-4083-b8db-04df9c8706bf" && u.Name == "ABC123 - BIM"));
            Assert.AreEqual(1, createFromTemplateResponse.GroupsCreated.Count(u =>
                u.Id == "c988438b-e5f0-4ed9-b788-0f4265115926" && u.Name == "ABC123 - External"));
        }

        [Test]
        public async Task CreateProjectFromTemplate_WhenParentFolderIdIsEmpty_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.ProjectFolders.CreateProjectFromTemplate(
                    parentFolderId: string.Empty,
                    templateFolderId: "c881b13e-3fac-4a88-88ea-7221ef2855d8",
                    folderName: "ABC123 - 123 Main St",
                    name: "Acme Widgets HQ",
                    status: "pending",
                    projectId: "ABC123"));

            Assert.IsTrue(exception.Message.Contains("parentFolderId"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task CreateProjectFromTemplate_WhenTemplateFolderIdIsEmpty_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.ProjectFolders.CreateProjectFromTemplate(
                    parentFolderId: "7c30e681-bf0d-462e-b6cf-7471844962df",
                    templateFolderId: string.Empty,
                    folderName: "ABC123 - 123 Main St",
                    name: "Acme Widgets HQ",
                    status: "pending",
                    projectId: "ABC123"));

            Assert.IsTrue(exception.Message.Contains("templateFolderId"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task CreateProjectFromTemplate_WhenFolderNameIsEmpty_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.ProjectFolders.CreateProjectFromTemplate(
                    parentFolderId: "7c30e681-bf0d-462e-b6cf-7471844962df",
                    templateFolderId: "c881b13e-3fac-4a88-88ea-7221ef2855d8",
                    folderName: string.Empty,
                    name: "Acme Widgets HQ",
                    status: "pending",
                    projectId: "ABC123"));

            Assert.IsTrue(exception.Message.Contains("folderName"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task CreateProjectFromTemplate_WhenNameIsEmpty_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.ProjectFolders.CreateProjectFromTemplate(
                    parentFolderId: "7c30e681-bf0d-462e-b6cf-7471844962df",
                    templateFolderId: "c881b13e-3fac-4a88-88ea-7221ef2855d8",
                    folderName: "ABC123 - 123 Main St",
                    name: string.Empty,
                    status: "pending",
                    projectId: "ABC123"));

            Assert.IsTrue(exception.Message.Contains("name"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task CreateProjectFromTemplate_WhenStatusIsEmpty_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.ProjectFolders.CreateProjectFromTemplate(
                    parentFolderId: "7c30e681-bf0d-462e-b6cf-7471844962df",
                    templateFolderId: "c881b13e-3fac-4a88-88ea-7221ef2855d8",
                    folderName: "ABC123 - 123 Main St",
                    name: "Acme Widgets HQ",
                    status: string.Empty,
                    projectId: "ABC123"));

            Assert.IsTrue(exception.Message.Contains("status"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task CreateProjectFromTemplate_WhenProjectIdIsEmpty_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.ProjectFolders.CreateProjectFromTemplate(
                    parentFolderId: "7c30e681-bf0d-462e-b6cf-7471844962df",
                    templateFolderId: "c881b13e-3fac-4a88-88ea-7221ef2855d8",
                    folderName: "ABC123 - 123 Main St",
                    name: "Acme Widgets HQ",
                    status: "pending",
                    projectId: string.Empty));

            Assert.IsTrue(exception.Message.Contains("projectId"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
