using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

using NUnit.Framework;
using System.IO;
using System.Net;

namespace Egnyte.Api.Tests.Files
{
    [TestFixture]
    public class ChunkedUploadTests
    {
        private const string Checksum = "6cb2785692b05c5eff397109457031bde7ab236982364cc7b51e319c67c463d7721c82c024ef3f74b9dff d388be6dc8120edc214e7d0eadaaf2c5e0eb44845a3";
        private const string UploadId = "a18967e9-dea4-4508-aeca-a0ecd2c971f1";
        private const string ETag = "9c4c2443-5dbc-4afa-8d04-5620a778093c";

        private const string CreateFileResponse = @"
        {
            ""checksum"":""6cb2785692b05c5eff397109457031bde7ab236982364cc7b51e319c67c463d7721c82c024ef3f74b9dff d388be6dc8120edc214e7d0eadaaf2c5e0eb44845a3"",
            ""group_id"":""a915703e-25f7-4905-9f46-2dbdcc94c681"",
            ""entry_id"":""9c4c2443-5dbc-4afa-8d04-5620a778093c""
        }";

        [Test]
        public async Task ChunkedUploadFirstChunk_ThrowsArgumentNullException_WhenNoPathSpecified()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Files.ChunkedUploadFirstChunk(
                    string.Empty,
                    new MemoryStream(Encoding.UTF8.GetBytes("file"))));

            Assert.IsTrue(exception.Message.Contains("path"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task ChunkedUploadFirstChunk_ThrowsArgumentNullException_WhenStreamIsEmpty()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Files.ChunkedUploadFirstChunk(
                    "path",
                    new MemoryStream()));

            Assert.IsTrue(exception.Message.Contains("file"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task ChunkedUploadFirstChunk_ReturnsCorrectResponse()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc = (request, cancellationToken) => Task.FromResult(GetResponseMessage());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var result = await egnyteClient.Files.ChunkedUploadFirstChunk(
                "path",
                new MemoryStream(Encoding.UTF8.GetBytes("file")));

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(Checksum, result.Checksum);
            Assert.AreEqual(1, result.ChunkNumber);
            Assert.AreEqual(UploadId, result.UploadId);
            Assert.AreEqual("https://acme.egnyte.com/pubapi/v1/fs-content-chunked/path", requestMessage.RequestUri.ToString());
        }

        [Test]
        public async Task ChunkedUploadNextChunk_ThrowsArgumentNullException_WhenNoPathSpecified()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Files.ChunkedUploadNextChunk(
                    string.Empty,
                    2,
                    UploadId,
                    new MemoryStream(Encoding.UTF8.GetBytes("file"))));

            Assert.IsTrue(exception.Message.Contains("path"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task ChunkedUploadNextChunk_ArgumentOutOfRangeException_WhenNumberIsLessThenTwo()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentOutOfRangeException>(
                () => egnyteClient.Files.ChunkedUploadNextChunk(
                    "path",
                    1,
                    UploadId,
                    new MemoryStream(Encoding.UTF8.GetBytes("file"))));

            Assert.IsTrue(exception.Message.Contains("chunkNumber"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task ChunkedUploadNextChunk_ArgumentNullException_WhenUploadIdIsEmpty()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Files.ChunkedUploadNextChunk(
                    "path",
                    2,
                    string.Empty,
                    new MemoryStream(Encoding.UTF8.GetBytes("file"))));

            Assert.IsTrue(exception.Message.Contains("chunkUploadId"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task ChunkedUploadNextChunk_ThrowsArgumentNullException_WhenStreamIsEmpty()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Files.ChunkedUploadNextChunk(
                    "path",
                    2,
                    UploadId,
                    new MemoryStream()));

            Assert.IsTrue(exception.Message.Contains("file"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task ChunkedUploadNextChunk_ReturnsCorrectResponse()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc = (request, cancellationToken) => Task.FromResult(GetNextResponseMessage());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var result = await egnyteClient.Files.ChunkedUploadNextChunk(
                "path",
                2,
                UploadId,
                new MemoryStream(Encoding.UTF8.GetBytes("file")));

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(Checksum, result.Checksum);
            Assert.AreEqual(2, result.ChunkNumber);
            Assert.AreEqual(UploadId, result.UploadId);
            Assert.AreEqual("https://acme.egnyte.com/pubapi/v1/fs-content-chunked/path", requestMessage.RequestUri.ToString());
        }
        
        [Test]
        public async Task ChunkedUploadLastChunk_ThrowsArgumentNullException_WhenNoPathSpecified()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Files.ChunkedUploadLastChunk(
                    string.Empty,
                    2,
                    UploadId,
                    new MemoryStream(Encoding.UTF8.GetBytes("file"))));

            Assert.IsTrue(exception.Message.Contains("path"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task ChunkedUploadLastChunk_ArgumentOutOfRangeException_WhenNumberIsLessThenTwo()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentOutOfRangeException>(
                () => egnyteClient.Files.ChunkedUploadLastChunk(
                    "path",
                    1,
                    UploadId,
                    new MemoryStream(Encoding.UTF8.GetBytes("file"))));

            Assert.IsTrue(exception.Message.Contains("chunkNumber"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task ChunkedUploadLastChunk_ArgumentNullException_WhenUploadIdIsEmpty()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Files.ChunkedUploadLastChunk(
                    "path",
                    2,
                    string.Empty,
                    new MemoryStream(Encoding.UTF8.GetBytes("file"))));

            Assert.IsTrue(exception.Message.Contains("chunkUploadId"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task ChunkedUploadLastChunk_ThrowsArgumentNullException_WhenStreamIsEmpty()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Files.ChunkedUploadLastChunk(
                    "path",
                    2,
                    UploadId,
                    new MemoryStream()));

            Assert.IsTrue(exception.Message.Contains("file"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task ChunkedUploadLastChunk_ReturnsCorrectResponse()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc = (request, cancellationToken) => Task.FromResult(GetLastResponseMessage());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var result = await egnyteClient.Files.ChunkedUploadLastChunk(
                "path",
                4,
                UploadId,
                new MemoryStream(Encoding.UTF8.GetBytes("file")));

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            var content = httpHandlerMock.GetRequestContentAsString();
            Assert.AreEqual(Checksum, result.Checksum);
            Assert.AreEqual("\"" + ETag + "\"", result.EntryId);
            Assert.AreEqual(new DateTime(2012, 08, 26, 5, 55, 29), result.LastModified);
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v1/fs-content-chunked/path",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual("file", content);
        }

        private HttpResponseMessage GetResponseMessage()
        {
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(string.Empty)
            };
            responseMessage.Headers.Add("X-Egnyte-Chunk-Sha512-Checksum", Checksum);
            responseMessage.Headers.Add("X-Egnyte-Chunk-Num", "1");
            responseMessage.Headers.Add("X-Egnyte-Upload-Id", UploadId);

            return responseMessage;
        }

        private HttpResponseMessage GetNextResponseMessage()
        {
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(string.Empty)
            };
            responseMessage.Headers.Add("X-Egnyte-Chunk-Sha512-Checksum", Checksum);
            responseMessage.Headers.Add("X-Egnyte-Chunk-Num", "2");
            responseMessage.Headers.Add("X-Egnyte-Upload-Id", UploadId);

            return responseMessage;
        }

        private HttpResponseMessage GetLastResponseMessage()
        {
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(CreateFileResponse)
            };
            responseMessage.Headers.Add("X-Sha512-Checksum", Checksum);
            responseMessage.Headers.ETag = new System.Net.Http.Headers.EntityTagHeaderValue("\"" + ETag + "\"");
            responseMessage.Content.Headers.Add("Last-Modified", "Sun, 26 Aug 2012 03:55:29 GMT");

            return responseMessage;
        }
    }
}
