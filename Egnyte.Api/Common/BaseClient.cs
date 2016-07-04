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

        internal BaseClient(HttpClient httpClient, string domain)
        {
            this.httpClient = httpClient;
            this.domain = domain;
        }

        internal UriBuilder BuildUri(string method, string query = null)
        {
            UriBuilder ub = new UriBuilder(baseSchema, string.Format(basePath, domain), basePort, method);
            if (query != null)
                ub.Query = query;

            return ub;
        }
    }
}
