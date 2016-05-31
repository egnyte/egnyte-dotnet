using System.Net.Http;

namespace Egnyte.Api.Common
{
    public class BaseClient
    {
        internal readonly HttpClient httpClient;

        internal readonly string domain;

        internal BaseClient(HttpClient httpClient, string domain)
        {
            this.httpClient = httpClient;
            this.domain = domain;
        }
    }
}
