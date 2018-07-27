using Egnyte.Api.Files;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Egnyte.Api.Tests.Files
{
    [TestFixture]
    public class UpdateFolderTests
    {
        public const string UpdateFolderResponse = @"
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
                ""restrict_move_delete"":true
            }";

        [Test]
        public async Task UpdateFolder_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(UpdateFolderResponse)
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var response = await egnyteClient.Files.UpdateFolder(
                    "Shared/MyDocument",
                    folderDescription: "This is new description set up at: " + DateTime.Now,
                    publicLinks: PublicLinksType.Files,
                    restrictMoveDelete: true,
                    emailPreferences: "{\"content_updates\": false, \"content_accessed\":true}");

            Assert.AreEqual("/Shared/MyDocument", response.Path);
            Assert.AreEqual("ApiTests2", response.Name);
            Assert.AreEqual(new DateTime(2016, 1, 6, 21, 3, 30), response.LastModified);
            Assert.AreEqual("b0330bd2-4290-47bc-a537-e5520dc7d320", response.FolderId);
            Assert.AreEqual("This is new description set up at: 2018-07-27 05:29:15", response.FolderDescription);
            Assert.True(response.IsFolder);
            Assert.AreEqual(PublicLinksType.Files, response.PublicLinks);
            Assert.True(response.RestrictMoveDelete);
        }

        [Test]
        public async Task UpdateFolder_ThrowsException_WhenPathIsNotSpecified()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentException>(
                () => egnyteClient.Files.UpdateFolder(string.Empty));

            Assert.IsNull(exception.InnerException);
            Assert.True(exception.Message.Contains("path"));
        }

        [Test]
        public async Task UpdateFolder_ThrowsException_WhenParametersAreNotSpecified()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentException>(
                () => egnyteClient.Files.UpdateFolder("Shared/MyDocument"));

            Assert.IsNull(exception.InnerException);
            Assert.AreEqual("None of the optional parameters were provided", exception.Message);
        }
    }
}
