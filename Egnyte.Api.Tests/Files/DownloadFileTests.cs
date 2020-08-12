﻿namespace Egnyte.Api.Tests.Files
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Egnyte.Api.Files;

    using NUnit.Framework;
    using System.Linq;
    [TestFixture]
    public class DownloadFileTests
    {
        const string Checksum = "6cb2785692b05c5eff397109457031bde7ab236982364cc7b51e319c67c463d7721c82c024ef3f74b9dff d388be6dc8120edc214e7d0eadaaf2c5e0eb44845a3";
        const string ETag = "9c4c2443-5dbc-4afa-8d04-5620a778093c";
        const string ContentType = "text/plain; charset=UTF-8";
        const int ContentLength = 126;

        [Test]
        public async Task DownloadFile_ReturnsCorrectFile()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) => Task.FromResult(GetResponseMessage());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var result = await egnyteClient.Files.DownloadFile("myFile").ConfigureAwait(false);
            var requestMessage = httpHandlerMock.GetHttpRequestMessage();

            Assert.AreEqual(3, result.Data.Length);
            Assert.AreEqual(0x20, result.Data[0]);
            Assert.AreEqual(0x21, result.Data[1]);
            Assert.AreEqual(0x22, result.Data[2]);
            Assert.AreEqual(Checksum, result.Checksum);
            Assert.AreEqual(new DateTimeOffset(2012, 08, 26, 5, 55, 29, TimeSpan.Zero).ToLocalTime().DateTime, result.LastModified);
            Assert.AreEqual("\"" + ETag + "\"", result.ETag);
            Assert.AreEqual(ContentType, result.ContentType);
            Assert.AreEqual(ContentLength, result.ContentLength);
            Assert.AreEqual(999, result.FullFileLength);
            Assert.AreEqual("https://acme.egnyte.com/pubapi/v1/fs-content/myFile", requestMessage.RequestUri.ToString());
        }

        [Test]
        public async Task DownloadFile_WithoutContentRange_ReturnsCorrectFile()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) => Task.FromResult(GetResponseWithoutContentRangeMessage());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var result = await egnyteClient.Files.DownloadFile("myFile").ConfigureAwait(false);
            var requestMessage = httpHandlerMock.GetHttpRequestMessage();

            Assert.AreEqual(3, result.Data.Length);
            Assert.AreEqual(0x20, result.Data[0]);
            Assert.AreEqual(0x21, result.Data[1]);
            Assert.AreEqual(0x22, result.Data[2]);
            Assert.AreEqual(Checksum, result.Checksum);
            Assert.AreEqual(new DateTimeOffset(2012, 08, 26, 5, 55, 29, TimeSpan.Zero).ToLocalTime().DateTime, result.LastModified);
            Assert.AreEqual("\"" + ETag + "\"", result.ETag);
            Assert.AreEqual(ContentType, result.ContentType);
            Assert.AreEqual(ContentLength, result.ContentLength);
            Assert.AreEqual(0, result.FullFileLength);
            Assert.AreEqual("https://acme.egnyte.com/pubapi/v1/fs-content/myFile", requestMessage.RequestUri.ToString());
        }


        [Test]
        public async Task DownloadFile_WithWrongRange_ThrowsArgumentOutOfRangeException()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) => Task.FromResult(this.GetResponseMessage());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentOutOfRangeException>(
                () => egnyteClient.Files.DownloadFile("myFile", new Range(1000, 0)));

            Assert.IsTrue(exception.Message.Contains("'From' parameter must be less or equal to 'to'"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task DownloadFile_WithRange_ReturnsCorrectFile()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) => Task.FromResult(GetResponseMessage());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var result = await egnyteClient.Files.DownloadFile("myFile", new Range(0, 100))
                .ConfigureAwait(false);

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            var rangeHeaders = requestMessage.Headers.GetValues("Range").ToArray();
            Assert.AreEqual(1, rangeHeaders.Length);
            Assert.AreEqual("bytes=0-100", rangeHeaders[0]);

            Assert.AreEqual(3, result.Data.Length);
            Assert.AreEqual(0x20, result.Data[0]);
            Assert.AreEqual(0x21, result.Data[1]);
            Assert.AreEqual(0x22, result.Data[2]);
            Assert.AreEqual(Checksum, result.Checksum);
            Assert.AreEqual(new DateTimeOffset(2012, 08, 26, 5, 55, 29, TimeSpan.Zero).ToLocalTime().DateTime, result.LastModified);
            Assert.AreEqual("\"" + ETag + "\"", result.ETag);
            Assert.AreEqual(ContentType, result.ContentType);
            Assert.AreEqual(ContentLength, result.ContentLength);
            Assert.AreEqual(999, result.FullFileLength);
        }
        
        [Test]
        public async Task DownloadFile_WithEntryId_ReturnsCorrectFile()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) => Task.FromResult(this.GetResponseMessage());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var result = await egnyteClient.Files.DownloadFile("myFile", entryId: ETag).ConfigureAwait(false);

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual("https://acme.egnyte.com/pubapi/v1/fs-content/myFile?entry_id=9c4c2443-5dbc-4afa-8d04-5620a778093c", requestMessage.RequestUri.ToString());

            Assert.AreEqual(3, result.Data.Length);
            Assert.AreEqual(0x20, result.Data[0]);
            Assert.AreEqual(0x21, result.Data[1]);
            Assert.AreEqual(0x22, result.Data[2]);
            Assert.AreEqual(Checksum, result.Checksum);
            Assert.AreEqual(new DateTimeOffset(2012, 08, 26, 5, 55, 29, TimeSpan.Zero).ToLocalTime().DateTime, result.LastModified);
            Assert.AreEqual("\"" + ETag + "\"", result.ETag);
            Assert.AreEqual(ContentType, result.ContentType);
            Assert.AreEqual(ContentLength, result.ContentLength);
            Assert.AreEqual(999, result.FullFileLength);
        }

        HttpResponseMessage GetResponseMessage()
        {
            var response = GetResponseWithoutContentRangeMessage();
            response.Content.Headers.Add("Content-Range", "bytes 0-100/999");

            return response;
        }

        HttpResponseMessage GetResponseWithoutContentRangeMessage()
        {
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new ByteArrayContent(new byte[] { 0x20, 0x21, 0x22 })
            };
            responseMessage.Headers.Add("X-Sha512-Checksum", Checksum);
            responseMessage.Headers.ETag = new System.Net.Http.Headers.EntityTagHeaderValue("\"" + ETag + "\"");
            responseMessage.Content.Headers.Add("Last-Modified", "Sun, 26 Aug 2012 05:55:29 GMT");
            responseMessage.Content.Headers.Add("Content-Type", ContentType);
            responseMessage.Content.Headers.Add("Content-Length", ContentLength.ToString(CultureInfo.InvariantCulture));

            return responseMessage;
        }
    }
}
