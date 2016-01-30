using System.Threading.Tasks;

namespace Egnyte.Api.Tests.Lniks
{
    using System.Net;
    using System.Net.Http;

    using NUnit.Framework;
    using System.Text;
    using Api.Links;
    using System;

    [TestFixture]
    public class GetLinkDetailsTests
    {
        const string GetLinkDetailsResponse = @"
            {
                ""path"": ""/Shared/johnd/fun.png"",
                ""type"": ""file"",
                ""accessibility"": ""anyone"",
                ""protection"": ""NONE"",
                ""recipients"": [""johnd@egnyte.com""],
                ""notify"": false,
                ""url"": ""https://test.egnyte.com/dl/jKI7Lx9VPA"",
                ""id"": ""jKI7Lx9VPA"",
                ""link_to_current"": false,
                ""creation_date"": ""2016-01-28"",
                ""created_by"": ""johnd""
            }";

        [Test]
        public async Task GetLinkDetails_ReturnsSuccess()
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
                                GetLinkDetailsResponse,
                                Encoding.UTF8,
                                "application/json")
                    });
            
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var linkDetails = await egnyteClient.Links.GetLinkDetails("jKI7Lx9VPA");

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v1/links/jKI7Lx9VPA",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual("/Shared/johnd/fun.png", linkDetails.Path);
            Assert.AreEqual(LinkType.File, linkDetails.Type);
            Assert.AreEqual(LinkAccessibility.Anyone, linkDetails.Accessibility);
            Assert.AreEqual("NONE", linkDetails.Protection);
            Assert.AreEqual(1, linkDetails.Recipients.Count);
            Assert.AreEqual("johnd@egnyte.com", linkDetails.Recipients[0]);
            Assert.IsFalse(linkDetails.Notify);
            Assert.AreEqual("https://test.egnyte.com/dl/jKI7Lx9VPA", linkDetails.Url);
            Assert.AreEqual("jKI7Lx9VPA", linkDetails.Id);
            Assert.IsFalse(linkDetails.LinkToCurrent);
            Assert.AreEqual(new DateTime(2016, 01, 28), linkDetails.CreationDate);
            Assert.AreEqual("johnd", linkDetails.CreatedBy);
        }

        [Test]
        public async Task GetLinkDetails_WithWrongId_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                            () => egnyteClient.Links.GetLinkDetails(string.Empty));

            Assert.IsTrue(exception.Message.Contains("linkId"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
