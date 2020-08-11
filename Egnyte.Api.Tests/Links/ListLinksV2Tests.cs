using Egnyte.Api.Links;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Egnyte.Api.Tests.Links
{
    [TestFixture]
    public class ListLinksV2Tests
    {
        private const string ListLinksResponseContent = @"
            {
                ""links"":[
                {
                    ""path"":""/Shared/MikTests/xbox.png"",
                    ""type"":""file"",
                    ""accessibility"":""anyone"",
                    ""protection"":""NONE"",
                    ""recipients"":[""mik.bialecki+test107@gmail.com""],
                    ""notify"":false,
                    ""url"":""https://test.egnyte.com/dl/PLeklqtBWN"",
                    ""id"":""PLeklqtBWN"",
                    ""link_to_current"":false,
                    ""creation_date"":""2016-01-28T06:01:17+0000"",
                    ""created_by"":""mik"",
                    ""resource_id"":""ebb76829-06b0-4e32-bec1-f7fea5b00311""},
               {
                    ""path"":""/Shared/MikTests/xbox2.png"",
                    ""type"":""file"",
                    ""accessibility"":""anyone"",
                    ""protection"":""NONE"",
                    ""recipients"":[""mik.bialecki@gmail.com""],
                    ""notify"":false,
                    ""url"":""https://test.egnyte.com/dl/jKI7Lx9VPE"",
                    ""id"":""jKI7Lx9VPE"",
                    ""link_to_current"":false,
                    ""creation_date"":""2016-01-28T06:01:17+0000"",
                    ""created_by"":""mik"",
                    ""resource_id"":""ebb76829-06b0-4e32-bec1-f7fea5b00311"",
                    ""expiry_date"":""2020-01-28T06:01:17+0000"",
                    ""expiry_clicks"":""6""}]
            ,""count"":2}";

        [Test]
        public async Task ListLinksV2_ReturnsSuccess()
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
                                ListLinksResponseContent,
                                Encoding.UTF8,
                                "application/json")
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var linksList = await egnyteClient.Links.ListLinksV2();

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v2/links",
                requestMessage.RequestUri.ToString());

            Assert.AreEqual(2, linksList.Count);
            var firstLink = linksList.Links.First();

            Assert.AreEqual("ebb76829-06b0-4e32-bec1-f7fea5b00311", firstLink.ResourceId);
            Assert.AreEqual("/Shared/MikTests/xbox.png", firstLink.Path);
            Assert.AreEqual(LinkType.File, firstLink.Type);
            Assert.AreEqual(LinkAccessibility.Anyone, firstLink.Accessibility);
            Assert.AreEqual(ProtectionType.None, firstLink.Protection);
            Assert.AreEqual(1, firstLink.Recipients.Count);
            Assert.AreEqual("mik.bialecki+test107@gmail.com", firstLink.Recipients.First());
            Assert.AreEqual(false, firstLink.Notify);
            Assert.AreEqual("https://test.egnyte.com/dl/PLeklqtBWN", firstLink.Url);
            Assert.AreEqual("PLeklqtBWN", firstLink.Id);
            Assert.AreEqual(false, firstLink.LinkToCurrent);
            Assert.AreEqual(new DateTime(2016, 01, 28, 6, 01, 17, DateTimeKind.Utc).ToLocalTime(), firstLink.CreationDate);
            Assert.AreEqual("mik", firstLink.CreatedBy);
            Assert.AreEqual(default(DateTime), firstLink.ExpiryDate);
            Assert.AreEqual(0, firstLink.ExpiryClicks);

            var secondLink = linksList.Links[1];
            Assert.AreEqual("ebb76829-06b0-4e32-bec1-f7fea5b00311", secondLink.ResourceId);
            Assert.AreEqual("/Shared/MikTests/xbox2.png", secondLink.Path);
            Assert.AreEqual(LinkType.File, secondLink.Type);
            Assert.AreEqual(LinkAccessibility.Anyone, secondLink.Accessibility);
            Assert.AreEqual(ProtectionType.None, secondLink.Protection);
            Assert.AreEqual(1, secondLink.Recipients.Count);
            Assert.AreEqual("mik.bialecki@gmail.com", secondLink.Recipients.First());
            Assert.AreEqual(false, secondLink.Notify);
            Assert.AreEqual("https://test.egnyte.com/dl/jKI7Lx9VPE", secondLink.Url);
            Assert.AreEqual("PLeklqtBWN", firstLink.Id);
            Assert.AreEqual(false, secondLink.LinkToCurrent);
            Assert.AreEqual(new DateTime(2016, 01, 28, 6, 01, 17, DateTimeKind.Utc).ToLocalTime(), secondLink.CreationDate);
            Assert.AreEqual("mik", secondLink.CreatedBy);
            Assert.AreEqual(new DateTime(2020, 01, 28, 6, 01, 17, DateTimeKind.Utc).ToLocalTime(), secondLink.ExpiryDate);
            Assert.AreEqual(6, secondLink.ExpiryClicks);
        }

        [Test]
        public async Task ListLinksV2_WithAllParameters_ReturnsSuccess()
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
                                ListLinksResponseContent,
                                Encoding.UTF8,
                                "application/json")
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var linksList = await egnyteClient.Links.ListLinksV2(
                "shared/",
                "johnd",
                new DateTime(2016, 05, 01),
                new DateTime(2016, 01, 01),
                Api.Links.LinkType.Folder,
                Api.Links.LinkAccessibility.Recipients,
                100,
                15);

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                @"https://acme.egnyte.com/pubapi/v2/links?path=shared/&username=johnd&created_before=2016-05-01&created_after=2016-01-01&type=folder&accessibility=recipients&offset=100&count=15",
                requestMessage.RequestUri.ToString());
        }
    }
}