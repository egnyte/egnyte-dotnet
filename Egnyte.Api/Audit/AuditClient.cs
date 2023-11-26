namespace Egnyte.Api.Audit
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Egnyte.Api.Common;
    using System.Collections.Generic;
    using System.Linq;

    public class AuditClient : BaseClient
    {
        const string AuditReportMethod = "/pubapi/v1/audit";
        const string AuditStreamingMethod = "/pubapi/v2/audit/stream";

        internal AuditClient(HttpClient httpClient, string domain = "", string host = "") : base(httpClient, domain, host) { }

        /// <summary>
        /// Access streaming version of audit reporting.
        /// </summary>
        /// <param name="startDate">Required if nextCursor not specified. Start of date range for the initial set of audit events. The start date should be within the last 7 days (from now). To retrieve past audit events outside of the 7 day window, it is needed to use Audit Reporting API v1.</param>
        /// <param name="endDate">Optional. End of date range for the initial set of audit events.</param>
        /// <param name="auditTypes">Allows to receive only specific types of audit events. If multiple types of events are required, it is recommended to receive all the required types (specifying the list of types in this filter) and then process them on the client as required. Allows filtering audit events by type based on the list of audit event types.</param>
        /// <param name="nextCursor">Iteration pointer for a following (not the initial) request. A cursor is returned in response to the initial request and then every following request generates a new cursor to be used in the next request.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<AuditV2ReportResponse> GetAuditV2Report(
            DateTime? startDate = null,
            DateTime? endDate = null,
            List<AuditV2Type> auditTypes = null,
            string nextCursor = null)
        {
            if (startDate == null && nextCursor == null)
            {
                throw new ArgumentException("Either 'startDate' or 'nextCursor' must be specified.", nameof(nextCursor));
            }

            var uriBuilder = BuildUri(AuditStreamingMethod);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
            {
                Content = new StringContent(
                    GetAuditV2ReportContent(
                        startDate,
                        endDate,
                        auditTypes,
                        nextCursor),
                    Encoding.UTF8,
                    "application/json")
            };

            var serviceHandler = new ServiceHandler<AuditV2ReportResponse>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return response.Data;
        }

        string GetAuditV2ReportContent(
            DateTime? startDate = null,
            DateTime? endDate = null,
            List<AuditV2Type> auditTypes = null,
            string nextCursor = null)
        {
            var builder = new StringBuilder();

            if (nextCursor != null)
            {
                builder
                    .Append("{")
                    .Append(string.Format("\"nextCursor\": \"{0}\"", nextCursor))
                    .Append("}");
            }
            else
            {
                builder
                    .Append("{")
                    .Append(string.Format("\"startDate\": \"{0:yyyy-MM-ddTHH:mm:ssZ}\"", startDate));

                if (endDate != null)
                {
                    builder
                        .Append(",")
                        .Append(string.Format("\"endDate\": \"{0:yyyy-MM-ddTHH:mm:ssZ}\"", endDate));
                }

                if (auditTypes != null)
                {
                    string auditsContent = "["
                        + string.Join(",", auditTypes.Select(e => "\"" + e.ToString() + "\""))
                        + "]";
                    builder
                        .Append(",")
                        .Append("\"auditType\": " + auditsContent);
                }

                builder.Append("}");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Creates login audit report
        /// </summary>
        /// <param name="format">Required. Determines format of audit report data</param>
        /// <param name="startDate">Required. Start of date range for report</param>
        /// <param name="endDate">Required. End of date range for report</param>
        /// <param name="events">Required. List of events to report on. At least one event must be specified</param>
        /// <param name="accessPoints">Optional. List of Egnyte access points covered by report.
        /// If not specified or empty then report will cover all access points</param>
        /// <param name="users">Optional. List of usernames to report on.
        /// If not specified or empty then report will cover all users</param>
        /// <returns>Id fo an audit report</returns>
        public async Task<string> CreateLoginAuditReport(
            AuditReportFormat format,
            DateTime startDate,
            DateTime endDate,
            List<string> events,
            List<AuditReportAccessPoint> accessPoints = null,
            List<string> users = null)
        {
            if (events == null || events.Count < 1)
            {
                throw new ArgumentException("At least one event must be specified.", nameof(events));
            }

            var uriBuilder = BuildUri(AuditReportMethod + "/logins");
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
            {
                Content = new StringContent(
                    GetCreateLoginAuditReportContent(
                        format,
                        startDate,
                        endDate,
                        events,
                        accessPoints,
                        users),
                    Encoding.UTF8,
                    "application/json")
            };
            
            var serviceHandler = new ServiceHandler<CreateLoginAuditReportResponse>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return response.Data.Id;
        }

        string GetCreateLoginAuditReportContent(
            AuditReportFormat format,
            DateTime startDate,
            DateTime endDate,
            List<string> events,
            List<AuditReportAccessPoint> accessPoints = null,
            List<string> users = null)
        {
            var eventsContent = "["
                    + string.Join(",", events.Select(e => "\"" + e + "\""))
                    + "]";

            var builder = new StringBuilder();
            builder
                .Append("{")
                .Append("\"format\": \"" + MapAuditReportFormat(format) + "\",")
                .Append(string.Format("\"date_start\": \"{0:yyyy-MM-ddTHH:mm:ssZ}\",", startDate))
                .Append(string.Format("\"date_end\": \"{0:yyyy-MM-ddTHH:mm:ssZ}\",", endDate))
                .Append("\"events\": " + eventsContent);

            if (accessPoints != null && accessPoints.Count > 0)
            {
                var accessPointsContent = "["
                + string.Join(",", accessPoints.Select(a => "\"" + MapAuditReportAccessPoint(a) + "\""))
                + "]";
                builder.Append(",\"access_points\": " + accessPointsContent);
            }

            if (users != null && users.Count > 0)
            {
                var usersContent = "["
                    + string.Join(",", users.Select(u => "\"" + u + "\""))
                    + "]";
                builder.Append(",\"users\": " + usersContent);
            }
            
            builder.Append("}");

            return builder.ToString();
        }

        string MapAuditReportFormat(AuditReportFormat format)
        {
            if (format == AuditReportFormat.CSV)
            {
                return "csv";
            }

            return "json";
        }

        string MapAuditReportAccessPoint(AuditReportAccessPoint accessPoint)
        {
            switch (accessPoint)
            {
                case AuditReportAccessPoint.Web:
                    return "web";
                case AuditReportAccessPoint.Mobile:
                    return "mobile";
                default:
                    return "ftp";
            }
        }
    }
}
