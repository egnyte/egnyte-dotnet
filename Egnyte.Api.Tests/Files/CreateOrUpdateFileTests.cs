using System.Threading.Tasks;

namespace Egnyte.Api.Tests.Files
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using NUnit.Framework;

    [TestFixture]
    public class CreateOrUpdateFileTests
    {
        private const string Checksum = "6cb2785692b05c5eff397109457031bde7ab236982364cc7b51e319c67c463d7721c82c024ef3f74b9dff d388be6dc8120edc214e7d0eadaaf2c5e0eb44845a3";
        private const string ETag = "9c4c2443-5dbc-4afa-8d04-5620a778093c";

        private const string CreateFileResponse = @"
        {
            ""checksum"":""6cb2785692b05c5eff397109457031bde7ab236982364cc7b51e319c67c463d7721c82c024ef3f74b9dff d388be6dc8120edc214e7d0eadaaf2c5e0eb44845a3"",
            ""group_id"":""a915703e-25f7-4905-9f46-2dbdcc94c681"",
            ""entry_id"":""9c4c2443-5dbc-4afa-8d04-5620a778093c""
        }";

        [Test]
        public async Task CreateOrUpdateFile_ThrowsArgumentNullException_WhenNoPathSpecified()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Files.CreateOrUpdateFile(
                    string.Empty,
                    new MemoryStream(Encoding.UTF8.GetBytes("file"))));

            Assert.IsTrue(exception.Message.Contains("path"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task CreateOrUpdateFile_ThrowsArgumentNullException_WhenStreamIsEmpty()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Files.CreateOrUpdateFile(
                    "path",
                    new MemoryStream()));

            Assert.IsTrue(exception.Message.Contains("file"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task CreateOrUpdateFile_ReturnsCorrectResponse()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc = (request, cancellationToken) => Task.FromResult(this.GetResponseMessage());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var result = await egnyteClient.Files.CreateOrUpdateFile(
                "path",
                new MemoryStream(Encoding.UTF8.GetBytes("file")));

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            var content = httpHandlerMock.GetRequestContentAsString();
            Assert.AreEqual(Checksum, result.Checksum);
            Assert.AreEqual("\"" + ETag + "\"", result.EntryId);
            Assert.AreEqual(new DateTimeOffset(2012, 08, 26, 5, 55, 29, TimeSpan.Zero).ToLocalTime().DateTime, result.LastModified);
            Assert.AreEqual("https://acme.egnyte.com/pubapi/v1/fs-content/path", requestMessage.RequestUri.ToString());
            Assert.AreEqual("file", content);
        }

        private HttpResponseMessage GetResponseMessage()
        {
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(CreateFileResponse)
            };
            responseMessage.Headers.Add("X-Sha512-Checksum", Checksum);
            responseMessage.Headers.ETag = new System.Net.Http.Headers.EntityTagHeaderValue("\"" + ETag + "\"");
            responseMessage.Content.Headers.Add("Last-Modified", "Sun, 26 Aug 2012 05:55:29 GMT");

            return responseMessage;
        }
    }
}
