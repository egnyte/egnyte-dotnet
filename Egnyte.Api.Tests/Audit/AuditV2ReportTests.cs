using Egnyte.Api.Audit;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Egnyte.Api.Tests.Search
{
    [TestFixture]
    public class AuditV2ReportTests
    {
        private const string RetryAfter = "20";
        private const string RateLimitMinute = "10";
        private const string RateLimitRemainingMinute = "0";
        private const string RateLimitHour = "100";
        private const string RateLimitRemainingHour = "46";

        const string GetAuditV2ReportRequestContent = @"
            {
                ""startDate"": ""2021-12-08T00:00:00Z"",
                ""endDate"": ""2021-12-10T00:00:00Z"",
                ""auditType"": [
                    ""FILE_AUDIT"",
                    ""USER_AUDIT"",
                    ""GROUP_AUDIT""
                ]
            }";

        const string GetAuditV2ReportRequestContentUsingCursor = @"
            {
	            ""nextCursor"": ""QmlnVGFibGVLZXk=""
            }";

        const string GetAuditV2ReportResponseContent = @"
            {
                ""nextCursor"": ""AAN_lwABAX1zZe9AAAAAAAAAAAAAAAAAAAAAAA"",
                ""events"":
                [
                    { ""date"":1638936585716,
                        ""sourcePath"":""/Shared/Departments/Marketing/Branding/Logo.jpg"",
                        ""targetPath"":""N/A"",
                        ""user"":""Jack Smith ( jsmith@company.com )"",
                        ""userId"":""101"",
                        ""action"":""Preview"",
                        ""access"":""Web UI"",
                        ""ipAddress"":""173.226.89.189"",
                        ""actionInfo"":"""",
                        ""auditSource"":""FILE_AUDIT""
                        },
                    { ""date"":1638937051697,
                    ""sourcePath"":""/Shared/Departments/Engineering/ProductY/Photo.png"",
                    ""targetPath"":""N/A"",
                    ""user"":""Adam Jackson ( ajackson@company.com )"",
                    ""userId"":""146"",
                    ""action"":""Upload"",
                    ""access"":""Web UI"",
                    ""ipAddress"":""172.8.18.17"",
                    ""actionInfo"":"""",
                    ""auditSource"":""FILE_AUDIT""
                    },
                    { ""date"":1638940605824,
                    ""actor"":""Jennifer Watkins ( jwatkins@company.com )"",
                    ""subject"":""Paul Chen ( pchen@company.com )"",
                    ""action"":""Disable"",
                    ""actionInfo"":"""",
                    ""source"":""Web UI"",
                    ""auditSource"":""USER_AUDIT""
                    },  
                { ""date"":1638942414189,
                    ""actor"":""Jennifer Watkins ( jwatkins@company.com )"",
                    ""group"":""Engineering"",
                    ""action"":""Create"",
                    ""actionInfo"":"""",
                    ""source"":""Web UI"",
                    ""auditSource"":""GROUP_AUDIT""
                    }
                ],
                ""moreEvents"":true
            }";

        [Test]
        public async Task GetAuditV2Report_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(GetAuditV2ReportResponseContent,
                                Encoding.UTF8,
                                "application/json")
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var auditReportResult = await egnyteClient.Audit.GetAuditV2Report(
                new DateTime(2021, 12, 08),
                new DateTime(2021, 12, 10),
                new List<AuditV2Type> { AuditV2Type.FILE_AUDIT, AuditV2Type.USER_AUDIT, AuditV2Type.GROUP_AUDIT });

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v2/audit/stream",
                requestMessage.RequestUri.ToString());

            Assert.AreEqual(HttpMethod.Post, requestMessage.Method);
            Assert.AreEqual("AAN_lwABAX1zZe9AAAAAAAAAAAAAAAAAAAAAAA", auditReportResult.NextCursor);
            Assert.AreEqual(4, auditReportResult.Events.Count);

            Assert.AreEqual(1638936585716, auditReportResult.Events[0].Date);
            Assert.AreEqual("/Shared/Departments/Marketing/Branding/Logo.jpg", auditReportResult.Events[0].SourcePath);
            Assert.AreEqual("N/A", auditReportResult.Events[0].TargetPath);
            Assert.AreEqual("Jack Smith ( jsmith@company.com )", auditReportResult.Events[0].User);
            Assert.AreEqual("101", auditReportResult.Events[0].UserId);
            Assert.AreEqual("Preview", auditReportResult.Events[0].Action);
            Assert.AreEqual("Web UI", auditReportResult.Events[0].Access);
            Assert.AreEqual("173.226.89.189", auditReportResult.Events[0].IpAddress);
            Assert.AreEqual("", auditReportResult.Events[0].ActionInfo);
            Assert.AreEqual(AuditV2Type.FILE_AUDIT.ToString(), auditReportResult.Events[0].AuditSource);

            Assert.AreEqual(true, auditReportResult.MoreEvents);

            var requestContent = httpHandlerMock.GetRequestContentAsString();
            Assert.AreEqual(
                TestsHelper.RemoveWhitespaces(GetAuditV2ReportRequestContent),
                TestsHelper.RemoveWhitespaces(requestContent)); 
        }

        [Test]
        public async Task GetAuditV2Report_WithCursor_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(GetAuditV2ReportResponseContent,
                                Encoding.UTF8,
                                "application/json")
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var auditReportResult = await egnyteClient.Audit.GetAuditV2Report(
                new DateTime(2021, 12, 08),
                new DateTime(2021, 12, 10),
                new List<AuditV2Type> { AuditV2Type.FILE_AUDIT, AuditV2Type.USER_AUDIT, AuditV2Type.GROUP_AUDIT },
                "QmlnVGFibGVLZXk=");

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v2/audit/stream",
                requestMessage.RequestUri.ToString());

            Assert.AreEqual(HttpMethod.Post, requestMessage.Method);
            Assert.AreEqual("AAN_lwABAX1zZe9AAAAAAAAAAAAAAAAAAAAAAA", auditReportResult.NextCursor);
            Assert.AreEqual(4, auditReportResult.Events.Count);

            Assert.AreEqual(true, auditReportResult.MoreEvents);

            var requestContent = httpHandlerMock.GetRequestContentAsString();
            Assert.AreEqual(
                TestsHelper.RemoveWhitespaces(GetAuditV2ReportRequestContentUsingCursor),
                TestsHelper.RemoveWhitespaces(requestContent));
        }

        [Test]
        public async Task GetAuditV2Report_WhenStartDateAndNextCursorAreEmpty_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentException>(
                            () => egnyteClient.Audit.GetAuditV2Report(
                            auditTypes: new List<AuditV2Type> { AuditV2Type.FILE_AUDIT, AuditV2Type.USER_AUDIT, AuditV2Type.GROUP_AUDIT }));

            Assert.IsTrue(exception.Message.Contains("nextCursor"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task GetAuditV2Report_ThrowsAuditV2RateLimitExceededException_WhenAccountOverAuditV2Limit()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);
            const string Content = "<h1>Developer Over AuditV2 Rate Limit</h1>";
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = (HttpStatusCode)429,
                Content = new StringContent(Content)
            };
            responseMessage.Headers.Add("retry-after", RetryAfter);
            responseMessage.Headers.Add("x-ratelimit-limit-minute", RateLimitMinute);
            responseMessage.Headers.Add("x-ratelimit-remaining-minute", RateLimitRemainingMinute);
            responseMessage.Headers.Add("x-ratelimit-limit-hour", RateLimitHour);
            responseMessage.Headers.Add("x-ratelimit-remaining-hour", RateLimitRemainingHour);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(responseMessage);

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<AuditV2RateLimitExceededException>(
                () => egnyteClient.Audit.GetAuditV2Report(nextCursor: "QmlnVGFibGVLZXk="));

            Assert.IsNull(exception.InnerException);
            Assert.AreEqual("Audit V2 Report stream over rate limit", exception.Message);

            Assert.AreEqual(RetryAfter, exception.RetryAfter);
            Assert.AreEqual(RateLimitMinute, exception.RateLimitMinute);
            Assert.AreEqual(RateLimitRemainingMinute, exception.RateLimitRemainingMinute);
            Assert.AreEqual(RateLimitHour, exception.RateLimitHour);
            Assert.AreEqual(RateLimitRemainingHour, exception.RateLimitRemainingHour);
        }
    }
}
