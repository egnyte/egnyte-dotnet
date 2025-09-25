using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Egnyte.Api.Tests.Links
{
    [TestFixture]
    public class ListLinksTests
    {
        const string ListLinksResponseContent = @"
            {
              ""ids"":[
                ""owTMm8H8Sg"",
                ""KsiryUUgEo"",
                ""ZJ8s7xmil5"",
                ""hhkfvX6nq8"",
                ""Kjw93J55sm""
              ],
              ""offset"":0,
              ""count"":0,
              ""total_count"":5
            }";

        [Test]
        public async Task ListLinks_ReturnsSuccess()
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
            var linksList = await egnyteClient.Links.ListLinks();

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v1/links",
                requestMessage.RequestUri.ToString());

            Assert.AreEqual(0, linksList.Count);
            Assert.AreEqual(5, linksList.TotalCount);
            Assert.AreEqual(0, linksList.Offset);
            Assert.AreEqual(5, linksList.Ids.Count);

            Assert.IsTrue(linksList.Ids.Contains("owTMm8H8Sg"));
            Assert.IsTrue(linksList.Ids.Contains("KsiryUUgEo"));
            Assert.IsTrue(linksList.Ids.Contains("ZJ8s7xmil5"));
            Assert.IsTrue(linksList.Ids.Contains("hhkfvX6nq8"));
            Assert.IsTrue(linksList.Ids.Contains("Kjw93J55sm"));
        }

        [Test]
        public async Task ListLinks_WithAllParameters_ReturnsSuccess()
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
            var linksList = await egnyteClient.Links.ListLinks(
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
                @"https://acme.egnyte.com/pubapi/v1/links?path=shared/&username=johnd&created_before=2016-05-01&created_after=2016-01-01&type=folder&accessibility=recipients&offset=100&count=15",
                requestMessage.RequestUri.ToString());
        }

        [Test]
        public async Task ListLinks_WithSpecialCharsInPath_EncodesPathInQuery()
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
            var linksList = await egnyteClient.Links.ListLinks(
                "/Shared/myFolder + anotherFolder");

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v1/links?path=/Shared/myFolder%20%2B%20anotherFolder",
                requestMessage.RequestUri.AbsoluteUri);
            Assert.AreEqual(HttpMethod.Get, requestMessage.Method);
        }
    }
}
