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

        internal AuditClient(HttpClient httpClient, string domain) : base(httpClient, domain) { }

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
                    GetCrateLoginAuditReportContent(
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

        string GetCrateLoginAuditReportContent(
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
