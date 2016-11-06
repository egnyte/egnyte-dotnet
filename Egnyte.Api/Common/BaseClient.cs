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
                ? string.Format(basePath, domain)
                : host;

            UriBuilder ub = new UriBuilder(baseSchema, userHost, basePort, method);
            if (query != null)
                ub.Query = query;

            return ub;
        }
    }
}
