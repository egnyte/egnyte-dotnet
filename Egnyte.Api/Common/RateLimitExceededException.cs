using System;
using System.Collections.Generic;
using System.Text;

namespace Egnyte.Api.Common
{
    public class RateLimitExceededException : Exception
    {
        public RateLimitExceededException(Dictionary<string, string> headers)
            : base("Account over rate limit")
        {
            Allotted = GetHeaderValue(headers, "x-accesstoken-quota-allotted");
            Current = GetHeaderValue(headers, "x-accesstoken-quota-current");
            RetryAfter = GetHeaderValue(headers, "retry-after");
        }

        public string Allotted { get; set; }
        public string Current { get; set; }
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
