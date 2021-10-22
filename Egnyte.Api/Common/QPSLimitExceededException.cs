using System;
using System.Collections.Generic;
using System.Text;

namespace Egnyte.Api.Common
{
    public class QPSLimitExceededException : Exception
    {
        public QPSLimitExceededException(Dictionary<string, string> headers)
            : base("Account over QPS limit")
        {
            Allotted = GetHeaderValue(headers, "x-accesstoken-qps-allotted");
            Current = GetHeaderValue(headers, "x-accesstoken-qps-current");
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
