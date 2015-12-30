namespace Egnyte.Api.Tests.Files
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Egnyte.Api.Common;

    using NUnit.Framework;

    [TestFixture]
    public class FilesClientTests
    {
        private const string ListFolderResponse = @"
            {    
            ""name"": ""Documents"",
            ""count"": 1,
            ""offset"": 2,
            ""path"": ""/Shared/Documents"",
            ""folder_id"": ""f3066c91-245c-446d-85ac-bfb88196e4e8"",
            ""total_count"": 18,
            ""is_folder"": true,
            ""public_links"": ""files_folders"",
            ""restrict_move_delete"": true,
            ""allowed_file_link_types"" : [""domain"", ""recipients""],
            ""allowed_folder_link_types"" : [""anyone"", ""password""],
            ""files"":
                [{
                    ""checksum"": ""checksum1"",
                    ""size"": 1048147,
                    ""path"": ""/Shared/Documents/nice_image.jpg"",
                    ""name"": ""nice_image.jpg"",
                    ""locked"": true,
                    ""is_folder"": false,
                    ""entry_id"": ""8d0ee165-fafe-4da8-aea4-4dda4dc7440b"",
                    ""group_id"": ""f882c636-4a2b-49af-a29c-2ec6507a2a1f"",
                    ""last_modified"": ""Tue, 14 Apr 2015 09:25:21 GMT"",
                    ""uploaded_by"": ""mik"",
                    ""num_versions"": 2
                }],
            ""folders"":
                [{
                    ""name"": ""Test"",
                    ""path"": ""/Shared/Documents/Test"",
                    ""folder_id"": ""6a1c7f21-874e-44d0-9360-ca09eacf8553"",
                    ""is_folder"": true,
                    ""allowed_file_link_types"" : [""domain"", ""recipients""],
                    ""allowed_folder_link_types"" : [""anyone"", ""password""]
                },
                {
                    ""name"": ""Articles"",
                    ""path"": ""/Shared/Documents/Articles"",
                    ""folder_id"": ""429b22bf-a111-4f7d-8460-58223db92817"",
                    ""is_folder"": true,
                    ""allowed_file_link_types"" : [""domain"", ""recipients""],
                    ""allowed_folder_link_types"" : [""anyone"", ""password""]
                }]
            }";

        private const string ListFileResponse = @"
        {
            ""checksum"": ""checksum1"",
            ""size"": 4799,
            ""path"": ""/Shared/Documents/report.docx"",
            ""name"": ""report.docx"",
            ""versions"":
            [{
                ""checksum"": ""checksum2"",
                ""size"": 4494,
                ""is_folder"": false,
                ""entry_id"": ""58327e97-0f42-4ef8-ae1a-39d1af420aae"",
                ""last_modified"": ""Mon, 17 Aug 2015 10:20:11 GMT"",
                ""uploaded_by"": ""mik""
            },
            {
                ""checksum"": ""checksum3"",
                ""size"": 4682,
                ""is_folder"": false,
                ""entry_id"": ""36502d58-c620-41d0-8821-b6c294555ebe"",
                ""last_modified"": ""Mon, 17 Aug 2015 10:27:38 GMT"",
                ""uploaded_by"": ""miki""
            }],
            ""locked"": false,
            ""is_folder"": false,
            ""entry_id"": ""d1b8222a-a57e-4d36-8370-d79ad3f29ee7"",
            ""group_id"": ""c0c01799-df8b-4859-bcb1-0fb6a80fc9ac"",
            ""last_modified"": ""Mon, 17 Aug 2015 10:28:55 GMT"",
            ""uploaded_by"": ""mik"",
            ""num_versions"": 3
        }";

        [Test]
        public async void ListFileOrFolder_ReturnsCorrectFolder()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc = (request, cancellationToken) => Task.FromResult(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(ListFolderResponse)
                });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var result = await egnyteClient.Files.ListFileOrFolder("path");

            Assert.AreEqual(true, result.IsFolder);
            Assert.NotNull(result.AsFolder);
            Assert.AreEqual(null, result.AsFile);

            var folderMetadata = result.AsFolder;
            Assert.AreEqual("Documents", folderMetadata.Name);
            Assert.AreEqual(1, folderMetadata.Count);
            Assert.AreEqual(2, folderMetadata.Offset);
            Assert.AreEqual("/Shared/Documents", folderMetadata.Path);
            Assert.AreEqual("f3066c91-245c-446d-85ac-bfb88196e4e8", folderMetadata.FolderId);
            Assert.AreEqual(18, folderMetadata.TotalCount);
            Assert.AreEqual("files_folders", folderMetadata.PublicLinks);
            Assert.AreEqual(true, folderMetadata.RestrictMoveDelete);
            Assert.AreEqual(2, folderMetadata.AllowedFileLinkTypes.Length);
            Assert.AreEqual("domain", folderMetadata.AllowedFileLinkTypes[0]);
            Assert.AreEqual("recipients", folderMetadata.AllowedFileLinkTypes[1]);
            Assert.AreEqual(2, folderMetadata.AllowedFolderLinkTypes.Length);
            Assert.AreEqual("anyone", folderMetadata.AllowedFolderLinkTypes[0]);
            Assert.AreEqual("password", folderMetadata.AllowedFolderLinkTypes[1]);
            
            Assert.AreEqual(1, folderMetadata.Files.Count);
            Assert.AreEqual("checksum1", folderMetadata.Files[0].Checksum);
            Assert.AreEqual("/Shared/Documents/nice_image.jpg", folderMetadata.Files[0].Path);
            Assert.AreEqual("nice_image.jpg", folderMetadata.Files[0].Name);
            Assert.AreEqual(true, folderMetadata.Files[0].Locked);
            Assert.AreEqual("8d0ee165-fafe-4da8-aea4-4dda4dc7440b", folderMetadata.Files[0].EntryId);
            Assert.AreEqual("f882c636-4a2b-49af-a29c-2ec6507a2a1f", folderMetadata.Files[0].GroupId);
            Assert.AreEqual(new DateTime(2015, 4, 14, 9, 25, 21), folderMetadata.Files[0].LastModified);
            Assert.AreEqual("mik", folderMetadata.Files[0].UploadedBy);
            Assert.AreEqual(2, folderMetadata.Files[0].NumberOfVersions);

            Assert.AreEqual(2, folderMetadata.Folders.Count);
            Assert.AreEqual("Test", folderMetadata.Folders[0].Name);
            Assert.AreEqual("/Shared/Documents/Test", folderMetadata.Folders[0].Path);
            Assert.AreEqual("6a1c7f21-874e-44d0-9360-ca09eacf8553", folderMetadata.Folders[0].FolderId);
            Assert.AreEqual(2, folderMetadata.Folders[0].AllowedFileLinkTypes.Length);
            Assert.AreEqual("domain", folderMetadata.Folders[0].AllowedFileLinkTypes[0]);
            Assert.AreEqual("recipients", folderMetadata.Folders[0].AllowedFileLinkTypes[1]);
            Assert.AreEqual(2, folderMetadata.Folders[0].AllowedFolderLinkTypes.Length);
            Assert.AreEqual("anyone", folderMetadata.Folders[0].AllowedFolderLinkTypes[0]);
            Assert.AreEqual("password", folderMetadata.Folders[0].AllowedFolderLinkTypes[1]);

            Assert.AreEqual("Articles", folderMetadata.Folders[1].Name);
            Assert.AreEqual("/Shared/Documents/Articles", folderMetadata.Folders[1].Path);
            Assert.AreEqual("429b22bf-a111-4f7d-8460-58223db92817", folderMetadata.Folders[1].FolderId);
            Assert.AreEqual(2, folderMetadata.Folders[1].AllowedFileLinkTypes.Length);
            Assert.AreEqual("domain", folderMetadata.Folders[1].AllowedFileLinkTypes[0]);
            Assert.AreEqual("recipients", folderMetadata.Folders[1].AllowedFileLinkTypes[1]);
            Assert.AreEqual(2, folderMetadata.Folders[1].AllowedFolderLinkTypes.Length);
            Assert.AreEqual("anyone", folderMetadata.Folders[1].AllowedFolderLinkTypes[0]);
            Assert.AreEqual("password", folderMetadata.Folders[1].AllowedFolderLinkTypes[1]);
        }

        [Test]
        public async void ListFileOrFolder_ReturnsCorrectFile()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc = (request, cancellationToken) => Task.FromResult(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(ListFileResponse)
                });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var result = await egnyteClient.Files.ListFileOrFolder("path");

            Assert.AreEqual(false, result.IsFolder);
            Assert.NotNull(result.AsFile);
            Assert.AreEqual(null, result.AsFolder);

            var fileMetadata = result.AsFile;
            Assert.AreEqual("report.docx", fileMetadata.Name);
            Assert.AreEqual("checksum1", fileMetadata.Checksum);
            Assert.AreEqual(4799, fileMetadata.Size);
            Assert.AreEqual("/Shared/Documents/report.docx", fileMetadata.Path);
            Assert.AreEqual("d1b8222a-a57e-4d36-8370-d79ad3f29ee7", fileMetadata.EntryId);
            Assert.AreEqual("c0c01799-df8b-4859-bcb1-0fb6a80fc9ac", fileMetadata.GroupId);
            Assert.AreEqual(false, fileMetadata.Locked);
            Assert.AreEqual(new DateTime(2015, 8, 17, 10, 28, 55), fileMetadata.LastModified);
            Assert.AreEqual("mik", fileMetadata.UploadedBy);
            Assert.AreEqual(3, fileMetadata.NumberOfVersions);

            Assert.AreEqual(2, fileMetadata.Versions.Count);
            Assert.AreEqual("checksum2", fileMetadata.Versions[0].Checksum);
            Assert.AreEqual(4494, fileMetadata.Versions[0].Size);
            Assert.AreEqual("58327e97-0f42-4ef8-ae1a-39d1af420aae", fileMetadata.Versions[0].EntryId);
            Assert.AreEqual(new DateTime(2015, 8, 17, 10, 20, 11), fileMetadata.Versions[0].LastModified);
            Assert.AreEqual("mik", fileMetadata.Versions[0].UploadedBy);

            Assert.AreEqual("checksum3", fileMetadata.Versions[1].Checksum);
            Assert.AreEqual(4682, fileMetadata.Versions[1].Size);
            Assert.AreEqual("36502d58-c620-41d0-8821-b6c294555ebe", fileMetadata.Versions[1].EntryId);
            Assert.AreEqual(new DateTime(2015, 8, 17, 10, 27, 38), fileMetadata.Versions[1].LastModified);
            Assert.AreEqual("miki", fileMetadata.Versions[1].UploadedBy);
        }

        [Test]
        public async void ListFileOrFolder_WhenNotWellFormedAnswerIsReturned_ThrowsException()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);
            const string Content = "not well formed content";

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            Content = new StringContent(Content)
                        });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<EgnyteApiException>(
                () => egnyteClient.Files.ListFileOrFolder("path"));

            Assert.AreEqual(HttpStatusCode.InternalServerError, exception.StatusCode);
            Assert.IsNotNull(exception.InnerException);
            Assert.AreEqual(Content, exception.Message);
        }
    }
}
