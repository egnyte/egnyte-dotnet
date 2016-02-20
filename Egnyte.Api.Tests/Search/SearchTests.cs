using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Egnyte.Api.Tests.Search
{
    [TestFixture]
    public class SearchTests
    {
        const string SearchResponseContent = @"
            {
                ""results"":[{
                    ""name"":""LocalCloudPress.doc"",
                    ""path"":""/Shared/Documents/Sales/Proposals/LocalCloudPress.doc"",
                    ""type"":""application/msword"",
                    ""size"":28672,
                    ""snippet"":""Version5Version 45\nVersion 3\n\ngnyte brings its storage cloud closer to home\n\nFebruary 17, 2009\n\nhtt"",
                    ""entry_id"":""2c8e1083-47f8-4d57-94dc-fd05429b7ec3"",
                    ""last_modified"":""2015-01-14T22:19:29Z"",
                    ""uploaded_by"":""David Pfeffer"",
                    ""uploaded_by_username"":""david"",
                    ""num_versions"":1,
                    ""snippet_html"":""Version5Version 45\nVersion 3\n\ngnyte brings its storage cloud closer to home\n\nFebruary 17, 2009\n\nhtt"",
                    ""is_folder"":false
                }],
                ""total_count"":20,
                ""offset"":0,
                ""count"":1
            }";

        [Test]
        public async Task Search_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(
                                SearchResponseContent,
                                Encoding.UTF8,
                                "application/json")
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var searchResults = await egnyteClient.Search.Search("CloudPress");

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v1/search?query=CloudPress",
                requestMessage.RequestUri.ToString());

            Assert.AreEqual(20, searchResults.TotalCount);
            Assert.AreEqual(0, searchResults.Offset);
            Assert.AreEqual(1, searchResults.Count);

            Assert.AreEqual(1, searchResults.Results.Count);
            Assert.AreEqual("LocalCloudPress.doc", searchResults.Results[0].Name);
            Assert.AreEqual("/Shared/Documents/Sales/Proposals/LocalCloudPress.doc", searchResults.Results[0].Path);
            Assert.AreEqual("application/msword", searchResults.Results[0].Type);
            Assert.AreEqual(28672, searchResults.Results[0].Size);
            Assert.AreEqual(
                "Version5Version 45\nVersion 3\n\ngnyte brings its storage cloud closer to home\n\nFebruary 17, 2009\n\nhtt",
                searchResults.Results[0].Snippet);
            Assert.AreEqual(
                "Version5Version 45\nVersion 3\n\ngnyte brings its storage cloud closer to home\n\nFebruary 17, 2009\n\nhtt",
                searchResults.Results[0].SnippetHtml);
            Assert.AreEqual("2c8e1083-47f8-4d57-94dc-fd05429b7ec3", searchResults.Results[0].EntryId);
            Assert.AreEqual(new DateTime(2015, 01, 14, 22, 19, 29), searchResults.Results[0].LastModified);
            Assert.AreEqual(1, searchResults.Results[0].NumberOfVersions);
            Assert.IsFalse(searchResults.Results[0].IsFolder);
        }

        [Test]
        public async Task Search_WhenAllParametersAreSpecified_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(
                                SearchResponseContent,
                                Encoding.UTF8,
                                "application/json")
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var searchResults = await egnyteClient.Search.Search(
                "CloudPress",
                50,
                20,
                "Shared",
                new DateTime(2016, 02, 20, 21, 53, 12),
                new DateTime(2015, 02, 20, 21, 53, 12));

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v1/search?query=CloudPress&offset=50&count=20&folder=Shared&modified_before=2016-02-20T21:53:12Z&modified_after=2015-02-20T21:53:12Z",
                requestMessage.RequestUri.ToString());

            Assert.AreEqual(20, searchResults.TotalCount);
            Assert.AreEqual(0, searchResults.Offset);
            Assert.AreEqual(1, searchResults.Count);

            Assert.AreEqual(1, searchResults.Results.Count);
            Assert.AreEqual("LocalCloudPress.doc", searchResults.Results[0].Name);
            Assert.AreEqual("/Shared/Documents/Sales/Proposals/LocalCloudPress.doc", searchResults.Results[0].Path);
            Assert.AreEqual("application/msword", searchResults.Results[0].Type);
            Assert.AreEqual(28672, searchResults.Results[0].Size);
            Assert.AreEqual(
                "Version5Version 45\nVersion 3\n\ngnyte brings its storage cloud closer to home\n\nFebruary 17, 2009\n\nhtt",
                searchResults.Results[0].Snippet);
            Assert.AreEqual(
                "Version5Version 45\nVersion 3\n\ngnyte brings its storage cloud closer to home\n\nFebruary 17, 2009\n\nhtt",
                searchResults.Results[0].SnippetHtml);
            Assert.AreEqual("2c8e1083-47f8-4d57-94dc-fd05429b7ec3", searchResults.Results[0].EntryId);
            Assert.AreEqual(new DateTime(2015, 01, 14, 22, 19, 29), searchResults.Results[0].LastModified);
            Assert.AreEqual(1, searchResults.Results[0].NumberOfVersions);
            Assert.IsFalse(searchResults.Results[0].IsFolder);
        }
        
        [Test]
        public async Task Search_WhenQueryIsEmpty_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                            () => egnyteClient.Search.Search(string.Empty));

            Assert.IsTrue(exception.Message.Contains("query"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task Search_WhenOffsetIsOutOfRange_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentOutOfRangeException>(
                            () => egnyteClient.Search.Search("file", -1));

            Assert.IsTrue(exception.Message.Contains("offset"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task Search_WhenCountIsOutOfRange_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentOutOfRangeException>(
                            () => egnyteClient.Search.Search("file", count: 110));

            Assert.IsTrue(exception.Message.Contains("count"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
