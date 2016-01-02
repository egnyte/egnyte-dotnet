namespace Egnyte.Api.Common
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    public class ServiceHandler<T> where T : class 
    {
        private readonly HttpClient httpClient;

        public ServiceHandler(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<T> SendRequestAsync(HttpRequestMessage request)
        {
            var response = await this.httpClient.SendAsync(request);
            var rawContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(rawContent);
                }
                catch (Exception e)
                {
                    throw new EgnyteApiException(
                        rawContent,
                        response.StatusCode,
                        e);
                }
            }

            throw new EgnyteApiException(
                    rawContent,
                    response.StatusCode);
        }
    }
}
