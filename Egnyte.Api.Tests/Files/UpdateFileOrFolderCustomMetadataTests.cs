using Egnyte.Api.Files;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Egnyte.Api.Tests.Files
{
    [TestFixture]
    public class UpdateFileOrFolderCustomMetadataTests
    {
        public const string UpdateFolderCustomMetadataResponse = @"
            {
                ""name"":""ApiTests2"",
                ""lastModified"":1452110610000,
                ""path"":""/Shared/MyDocument"",
                ""folder_id"":""b0330bd2-4290-47bc-a537-e5520dc7d320"",
                ""folder_description"":""This is new description set up at: 2018-07-27 05:29:15"",
                ""parent_id"":""d28cfd3b-06fa-4d9a-991a-070f71c63529"",
                ""allow_links"":true,
                ""is_folder"":true,
                ""public_links"":""files"",
                ""restrict_move_delete"":true,
                ""allow_links"":false
            }";

        private const string UpdateFileCustomMetadataResponse = @"
        {
            ""uploaded"": ""1439806811000"",
            ""checksum"": ""checksum1"",
            ""size"": 4799,
            ""path"": ""/Shared/Documents/report.docx"",
            ""name"": ""report.docx"",
            ""locked"": false,
            ""is_folder"": false,
            ""entry_id"": ""d1b8222a-a57e-4d36-8370-d79ad3f29ee7"",
            ""group_id"": ""c0c01799-df8b-4859-bcb1-0fb6a80fc9ac"",
            ""last_modified"": ""Mon, 17 Aug 2015 10:28:55 GMT"",
            ""uploaded_by"": ""mik"",
            ""num_versions"": 1
        }";

        [Test]
        public async Task UpdateFileOrFolderCustomMetadata_ForFolder_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(UpdateFolderCustomMetadataResponse)
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var response = await egnyteClient.Files.UpdateFileOrFolderCustomMetadata(
                    "path",
                    "custom attributes",
                    new FileOrFolderCustomMetadataProperties
                    {
                        { "reviewed", false },
                        { "tags", "important" }
                    });

            Assert.IsTrue(response);

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v1/fs/ids/folder/b0330bd2-4290-47bc-a537-e5520dc7d320/properties/custom attributes",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual(
                "{\"reviewed\" : \"False\",\"tags\" : \"important\"}",
                httpHandlerMock.GetRequestContentAsString());
        }

        [Test]
        public async Task UpdateFileOrFolderCustomMetadata_ForFile_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                    Task.FromResult(
                        new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = new StringContent(UpdateFileCustomMetadataResponse)
                        });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var response = await egnyteClient.Files.UpdateFileOrFolderCustomMetadata(
                "path",
                "custom attributes",
                new FileOrFolderCustomMetadataProperties
                {
                    { "reviewed", false },
                    { "contentType", "application/vnd.openxmlformats-officedocument.wordprocessing" }
                });

            Assert.IsTrue(response);

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v1/fs/ids/file/c0c01799-df8b-4859-bcb1-0fb6a80fc9ac/properties/custom attributes",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual(
                "{\"reviewed\" : \"False\",\"contentType\" : \"application/vnd.openxmlformats-officedocument.wordprocessing\"}",
                httpHandlerMock.GetRequestContentAsString());
        }

        [Test]
        public async Task UpdateFileOrFolderCustomMetadata_ThrowsException_WhenPathIsNotSpecified()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentException>(
                () => egnyteClient.Files.UpdateFileOrFolderCustomMetadata(
                    string.Empty,
                    "custom attributes",
                    new FileOrFolderCustomMetadataProperties()));

            Assert.IsNull(exception.InnerException);
            Assert.True(exception.Message.Contains("path"));
        }

        [Test]
        public async Task UpdateFileOrFolderCustomMetadata_ThrowsException_WhenSectionNameIsNotSpecified()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentException>(
                () => egnyteClient.Files.UpdateFileOrFolderCustomMetadata(
                    "path",
                    string.Empty,
                    new FileOrFolderCustomMetadataProperties()));

            Assert.IsNull(exception.InnerException);
            Assert.True(exception.Message.Contains("sectionName"));
        }

        [Test]
        public async Task UpdateFileOrFolderCustomMetadata_ThrowsException_WhenPropertiesAreNotSpecified()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentException>(
                () => egnyteClient.Files.UpdateFileOrFolderCustomMetadata(
                    "path",
                    "custom attributes",
                    new FileOrFolderCustomMetadataProperties()));

            Assert.IsNull(exception.InnerException);
            Assert.True(exception.Message.Contains("None of the custom metadata properties were provided"));
        }
    }
}