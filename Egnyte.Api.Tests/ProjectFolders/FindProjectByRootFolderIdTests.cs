using NUnit.Framework;
using System;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;

namespace Egnyte.Api.Tests.ProjectFolders
{
    [TestFixture]
    public class FindProjectByRootFolderIdTests
    {
        const string FindProjectByRootFolderIdResponseContent = @"
           [
              {
                ""id"": ""a69bd625-1dc3-4dcf-98f5-8e3e3fbb0b29"",
                ""rootFolderId"": ""b133ce0d-2e79-4cf8-bf3a-8758f7b117b3"",
                ""name"": ""Acme Widgets HQ Redesign"",
                ""projectId"": ""ABC-123"",
                ""customerName"": ""Acme Widgets Co"",
                ""description"": ""Redesigned HQ for Acme Widgets"",
                ""location"": {
                  ""streetAddress1"": ""123 Main St."",
                  ""streetAddress2"": null,
                  ""city"": ""Anytown"",
                  ""state"": ""CA"",
                  ""country"": ""USA"",
                  ""postalCode"": ""99999""
                },
                ""status"": ""in-progress"",
                ""startDate"": ""2022-11-01T00:00:00.000+0000"",
                ""createdBy"": 4,
                ""lastUpdatedBy"": 4,
                ""creationTime"": ""2022-11-02T19:02:31.000+0000"",
                ""lastModifiedTime"": ""2022-11-02T12:09:50.000+0000""
              }
            ] ";

        [Test]
        public async Task FindProjectByRootFolderId_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                    Task.FromResult(
                        new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = new StringContent(FindProjectByRootFolderIdResponseContent)
                        });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var findProjectByRootFolderIdResponse = await egnyteClient.ProjectFolders.FindProjectByRootFolderId("b133ce0d-2e79-4cf8-bf3a-8758f7b117b3");
            var projectDetails = findProjectByRootFolderIdResponse.First();

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v2/project-folders/search",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual(HttpMethod.Post, requestMessage.Method);

            Assert.AreEqual(1, findProjectByRootFolderIdResponse.Count);
            Assert.AreEqual(1, findProjectByRootFolderIdResponse.Count);

            Assert.AreEqual("a69bd625-1dc3-4dcf-98f5-8e3e3fbb0b29", projectDetails.Id);
            Assert.AreEqual("b133ce0d-2e79-4cf8-bf3a-8758f7b117b3", projectDetails.RootFolderId);
            Assert.AreEqual("Acme Widgets HQ Redesign", projectDetails.Name);
            Assert.AreEqual("ABC-123", projectDetails.ProjectId);
            Assert.AreEqual("Acme Widgets Co", projectDetails.CustomerName);
            Assert.AreEqual("Redesigned HQ for Acme Widgets", projectDetails.Description);
            Assert.AreEqual("123 Main St.", projectDetails.Location.StreetAddress1);
            Assert.AreEqual(null, projectDetails.Location.StreetAddress2);
            Assert.AreEqual("Anytown", projectDetails.Location.City);
            Assert.AreEqual("CA", projectDetails.Location.State);
            Assert.AreEqual("USA", projectDetails.Location.Country);
            Assert.AreEqual("99999", projectDetails.Location.PostalCode);

            Assert.AreEqual("in-progress", projectDetails.Status);
            Assert.AreEqual(0, DateTime.Compare(new DateTime(2022, 11, 01, 0, 0, 0, DateTimeKind.Utc).ToLocalTime(), projectDetails.StartDate));
            Assert.AreEqual(4, projectDetails.CreatedBy);
            Assert.AreEqual(4, projectDetails.LastUpdatedBy);
            Assert.AreEqual(0, DateTime.Compare(new DateTime(2022, 11, 02, 19, 02, 31, DateTimeKind.Utc).ToLocalTime(), projectDetails.CreationTime));
            Assert.AreEqual(0, DateTime.Compare(new DateTime(2022, 11, 02, 12, 09, 50, DateTimeKind.Utc).ToLocalTime(), projectDetails.LastModifiedTime));
        }

        [Test]
        public async Task FindProjectByRootFolderId_WhenRootFolderIdIsEmpty_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.ProjectFolders.FindProjectByRootFolderId(rootFolderId: string.Empty));

            Assert.IsTrue(exception.Message.Contains("rootFolderId"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
