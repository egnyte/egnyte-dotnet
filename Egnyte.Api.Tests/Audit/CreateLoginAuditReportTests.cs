using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Egnyte.Api.Tests.Search
{
    [TestFixture]
    public class CreateLoginAuditReportTests
    {
        const string CreateLoginAuditReportRequestContent = @"
            {
                ""format"": ""json"",
                ""date_start"": ""2012-05-01T00:00:00Z"",
                ""date_end"": ""2012-05-20T00:00:00Z"",
                ""events"": [
                    ""logins"",
                    ""failed_attempts""
                ],
                ""access_points"": [
                    ""web"",
                    ""ftp""
                ],
                ""users"": [
                    ""jsmith"",
                    ""kjohnson""
                ]
            }";

        [Test]
        public async Task CreateLoginAuditReport_ReturnsSuccess()
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
                                "{ \"id\": \"12345678\",}",
                                Encoding.UTF8,
                                "application/json")
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var auditReportResult = await egnyteClient.Audit.CreateLoginAuditReport(
                Audit.AuditReportFormat.Json,
                new DateTime(2012, 05, 01),
                new DateTime(2012, 05, 20),
                new List<string> { "logins", "failed_attempts" },
                new List<Audit.AuditReportAccessPoint>
                {
                    Audit.AuditReportAccessPoint.Web,
                    Audit.AuditReportAccessPoint.FTP
                },
                new List<string> { "jsmith", "kjohnson" });

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v1/audit/logins",
                requestMessage.RequestUri.ToString());

            Assert.AreEqual(HttpMethod.Post, requestMessage.Method);
            Assert.AreEqual("12345678", auditReportResult);

            var requestContent = httpHandlerMock.GetRequestContentAsString();
            Assert.AreEqual(
                TestsHelper.RemoveWhitespaces(CreateLoginAuditReportRequestContent),
                TestsHelper.RemoveWhitespaces(requestContent));
        }

        [Test]
        public async Task CreateLoginAuditReport_WhenEventsAreEmpty_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentException>(
                            () => egnyteClient.Audit.CreateLoginAuditReport(
                                Audit.AuditReportFormat.Json,
                                new DateTime(2012, 05, 01),
                                new DateTime(2012, 05, 20),
                                new List<string>()));

            Assert.IsTrue(exception.Message.Contains("events"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
