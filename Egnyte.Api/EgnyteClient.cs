namespace Egnyte.Api
{
    using System.Net.Http;
    using System.Net.Http.Headers;

    using Egnyte.Api.Files;

    public class EgnyteClient
    {
        public EgnyteClient(string token, string domain, HttpClient httpClient = null)
        {
            httpClient = httpClient ?? new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Files = new FilesClient(httpClient, domain);
        }

        public FilesClient Files { get; private set; }
    }
}
