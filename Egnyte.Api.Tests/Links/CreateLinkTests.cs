using Egnyte.Api.Links;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Egnyte.Api.Tests.Links
{
    [TestFixture]
    public class CreateLinkTests
    {
        const string CreateLinkResponse = @"
            {
              ""links"": [
                {
                  ""id"": ""47b774f66f344a67"",
                  ""url"": ""https://domain.egnyte.com/h-s/20130717/47b774f66f344a67"",
                  ""recipients"": [
                      ""jsmith@acme.com""
                  ]
                },
                {
                  ""id"": ""47b774f66f344a68"",
                  ""url"": ""https://domain.egnyte.com/h-s/20130717/47b774f66f344a68"",
                  ""recipients"": [
                      ""mjones@acme.com""
                  ]
                }
              ],
              ""path"": ""/Shared/Documents/example.txt"",
              ""type"": ""file"",
              ""accessibility"": ""recipients"",
              ""notify"": true,
              ""link_to_current"": false,
              ""expiry_date"": ""2012-05-27"",
              ""creation_date"": ""2012-05-02"",
              ""created_by"": ""gbrown"" 
            }";

        const string CreateLinkRequestContent = @"
            {
                ""path"":""/Shared/Documents/example.txt"",
	            ""type"":""file"",
	            ""accessibility"": ""recipients"",
                ""send_email"": ""true"",
                ""recipients"": [""jsmith@acme.com"", ""mjones@acme.com""],
                ""message"": ""Great example"",
                ""copy_me"": ""false"",
                ""notify"": ""true"",
                ""link_to_current"": ""false"",
                ""expiry_date"": ""2012-05-27""
            }";

        [Test]
        public async Task CreateLink_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.Created,
                        Content = new StringContent(
                                CreateLinkResponse,
                                Encoding.UTF8,
                                "application/json")
                    });

            var newLink = GetNewLink();
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var existingLink = await egnyteClient.Links.CreateLink(newLink);

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual("https://acme.egnyte.com/pubapi/v1/links", requestMessage.RequestUri.ToString());
            Assert.AreEqual(newLink.Path, existingLink.Path);
            Assert.AreEqual(newLink.Type, existingLink.Type);
            Assert.AreEqual(newLink.Accessibility, existingLink.Accessibility);
            Assert.AreEqual(newLink.ExpiryDate, existingLink.ExpiryDate);
            Assert.AreEqual(newLink.LinkToCurrent, existingLink.LinkToCurrent);
            Assert.AreEqual(newLink.Notify, existingLink.Notify);
            Assert.AreEqual(new DateTime(2012, 05, 02), existingLink.CreationDate);
            Assert.AreEqual("gbrown", existingLink.CreatedBy);
            Assert.AreEqual(2, existingLink.Links.Count);

            var firstLink = existingLink.Links.FirstOrDefault(l => l.Id == "47b774f66f344a67");
            Assert.AreEqual(
                "https://domain.egnyte.com/h-s/20130717/47b774f66f344a67",
                firstLink.Url);
            Assert.AreEqual(1, firstLink.Recipients.Count);
            Assert.AreEqual("jsmith@acme.com", firstLink.Recipients[0]);

            var secondLink = existingLink.Links.FirstOrDefault(l => l.Id == "47b774f66f344a68");
            Assert.AreEqual(
                "https://domain.egnyte.com/h-s/20130717/47b774f66f344a68",
                secondLink.Url);
            Assert.AreEqual(1, secondLink.Recipients.Count);
            Assert.AreEqual("mjones@acme.com", secondLink.Recipients[0]);

            var content = httpHandlerMock.GetRequestContentAsString();
            Assert.AreEqual(
                TestsHelper.RemoveWhitespaces(CreateLinkRequestContent),
                TestsHelper.RemoveWhitespaces(content));
        }

        [Test]
        public async Task CreateLink_WithNullParameter_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                            () => egnyteClient.Links.CreateLink(null));

            Assert.IsTrue(exception.Message.Contains("link"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task CreateLink_WithoutPathSpecified_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var newLink = GetNewLink();
            newLink.Path = null;
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                            () => egnyteClient.Links.CreateLink(newLink));

            Assert.IsTrue(exception.Message.Contains("Path"));
            Assert.IsNull(exception.InnerException);
        }

        NewLink GetNewLink()
        {
            return new NewLink
            {
                Path = "/Shared/Documents/example.txt",
                Type = LinkType.File,
                Accessibility = LinkAccessibility.Recipients,
                SendEmail = true,
                Recipients = new List<string> { "jsmith@acme.com", "mjones@acme.com" },
                Message = "Great example",
                CopyMe = false,
                Notify = true,
                LinkToCurrent = false,
                ExpiryDate = new DateTime(2012, 05, 27)
            };
        }
    }
}
