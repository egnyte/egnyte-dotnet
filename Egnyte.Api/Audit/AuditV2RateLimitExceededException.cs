using System;
using System.Collections.Generic;
using System.Text;

namespace Egnyte.Api.Audit
{
    public class AuditV2RateLimitExceededException : Exception
    {
        public AuditV2RateLimitExceededException(Dictionary<string, string> headers)
            : base("Audit V2 Report stream over rate limit")
        {
            RateLimitMinute = GetHeaderValue(headers, "x-ratelimit-limit-minute");
            RateLimitRemainingMinute = GetHeaderValue(headers, "x-ratelimit-remaining-minute");
            RateLimitHour = GetHeaderValue(headers, "x-ratelimit-limit-hour");
            RateLimitRemainingHour = GetHeaderValue(headers, "x-ratelimit-remaining-hour");
            RetryAfter = GetHeaderValue(headers, "retry-after");
        }

        public string RateLimitMinute { get; set; }
        public string RateLimitRemainingMinute { get; set; }
        public string RateLimitHour { get; set; }
        public string RateLimitRemainingHour { get; set; }
        public string RetryAfter { get; set; }

        private string GetHeaderValue(Dictionary<string, string> headers, string headerName)
        {
            if (headers.ContainsKey(headerName) && !string.IsNullOrWhiteSpace(headers[headerName]))
            {
                return headers[headerName];
            }

            return string.Empty;
        }
    }
}
