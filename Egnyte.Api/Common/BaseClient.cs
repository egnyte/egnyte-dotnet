using System;
using System.Net.Http;

namespace Egnyte.Api.Common
{
    public class BaseClient
    {
        const string basePath = "{0}.egnyte.com";
        const string baseSchema = "https";
        const int basePort = 443;

        internal readonly HttpClient httpClient;

        internal readonly string domain;

        internal readonly string host;

        internal BaseClient(HttpClient httpClient, string domain = "", string host = "")
        {
            this.httpClient = httpClient;
            this.domain = domain;
            this.host = host;
        }

        internal UriBuilder BuildUri(string method, string query = null)
        {
            var userHost = string.IsNullOrWhiteSpace(host)
                ? (string.IsNullOrWhiteSpace(domain)
                    ? string.Empty
                    : (domain.Contains(".") ? domain : string.Format(basePath, domain)))
                : host;

            UriBuilder ub = new UriBuilder(baseSchema, userHost, basePort, method);
            if (query != null)
                ub.Query = query;

            return ub;
        }

        protected string EncodePathSegments(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }

            var trimmed = path.StartsWith("/", StringComparison.Ordinal) ? path.Substring(1) : path;
            var segments = trimmed.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < segments.Length; i++)
            {
                segments[i] = Uri.EscapeDataString(segments[i]);
            }
            return string.Join("/", segments);
        }
    }
}
