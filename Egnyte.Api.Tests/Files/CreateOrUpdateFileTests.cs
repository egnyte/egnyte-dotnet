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
        private const string CreateFileResponse = @"
        {
            ""checksum"":""cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e"",
            ""group_id"":""a915703e-25f7-4905-9f46-2dbdcc94c681"",
            ""entry_id"":""e305a490-f869-402b-8efb-7fc3f5c146e7""
        }";

        [Test]
        public async void CreateOrUpdateFile_ThrowsArgumentNullException_WhenNoPathSpecified()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Files.CreateOrUpdateFile(
                    string.Empty,
                    new MemoryStream(Encoding.UTF8.GetBytes("file"))));

            Assert.IsTrue(exception.Message.Contains("Parameter name: path"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async void CreateOrUpdateFile_ThrowsArgumentNullException_WhenStreamIsEmpty()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Files.CreateOrUpdateFile(
                    "path",
                    new MemoryStream()));

            Assert.IsTrue(exception.Message.Contains("Parameter name: file"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async void CreateOrUpdateFile_ReturnsCorrectResponse()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc = (request, cancellationToken) => Task.FromResult(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(CreateFileResponse)
                });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var result = await egnyteClient.Files.CreateOrUpdateFile(
                "path",
                new MemoryStream(Encoding.UTF8.GetBytes("file")));

            Assert.AreEqual("cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e", result.Checksum);
            Assert.AreEqual("a915703e-25f7-4905-9f46-2dbdcc94c681", result.GroupId);
            Assert.AreEqual("e305a490-f869-402b-8efb-7fc3f5c146e7", result.EntryId);
        }
    }
}
