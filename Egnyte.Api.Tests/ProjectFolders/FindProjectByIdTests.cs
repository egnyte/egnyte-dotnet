using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Egnyte.Api.Tests.ProjectFolders
{
    [TestFixture]
    public class FindProjectByIdTests
    {
        const string FindProjectByIdResponseContent = @"
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
            } ";

        [Test]
        public async Task FindProjectById_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                    Task.FromResult(
                        new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = new StringContent(FindProjectByIdResponseContent)
                        });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var projectByIdResponse = await egnyteClient.ProjectFolders.FindProjectById("ABC-123");

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v2/project-folders/ABC-123",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual(HttpMethod.Get, requestMessage.Method);

            Assert.AreEqual("a69bd625-1dc3-4dcf-98f5-8e3e3fbb0b29", projectByIdResponse.Id);
            Assert.AreEqual("b133ce0d-2e79-4cf8-bf3a-8758f7b117b3", projectByIdResponse.RootFolderId);
            Assert.AreEqual("Acme Widgets HQ Redesign", projectByIdResponse.Name);
            Assert.AreEqual("ABC-123", projectByIdResponse.ProjectId);
            Assert.AreEqual("Acme Widgets Co", projectByIdResponse.CustomerName);
            Assert.AreEqual("Redesigned HQ for Acme Widgets", projectByIdResponse.Description);
            Assert.AreEqual("123 Main St.", projectByIdResponse.Location.StreetAddress1);
            Assert.AreEqual(null, projectByIdResponse.Location.StreetAddress2);
            Assert.AreEqual("Anytown", projectByIdResponse.Location.City);
            Assert.AreEqual("CA", projectByIdResponse.Location.State);
            Assert.AreEqual("USA", projectByIdResponse.Location.Country);
            Assert.AreEqual("99999", projectByIdResponse.Location.PostalCode);

            Assert.AreEqual("in-progress", projectByIdResponse.Status);
            Assert.AreEqual(0, DateTime.Compare(new DateTime(2022, 11, 01, 0, 0, 0, DateTimeKind.Utc).ToLocalTime(), projectByIdResponse.StartDate));
            Assert.AreEqual(4, projectByIdResponse.CreatedBy);
            Assert.AreEqual(4, projectByIdResponse.LastUpdatedBy);
            Assert.AreEqual(0, DateTime.Compare(new DateTime(2022, 11, 02, 19, 02, 31, DateTimeKind.Utc).ToLocalTime(), projectByIdResponse.CreationTime));
            Assert.AreEqual(0, DateTime.Compare(new DateTime(2022, 11, 02, 12, 09, 50, DateTimeKind.Utc).ToLocalTime(), projectByIdResponse.LastModifiedTime));
        }

        [Test]
        public async Task FindProjectById_WhenProjectIdIsEmpty_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.ProjectFolders.FindProjectById(projectId: string.Empty));

            Assert.IsTrue(exception.Message.Contains("projectId"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
