namespace Egnyte.Api.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        public async Task<ServiceResponse<T>> SendRequestAsync(HttpRequestMessage request)
        {
            request.RequestUri = ApplyAdditionalUrlMapping(request.RequestUri);
            var response = await this.httpClient.SendAsync(request).ConfigureAwait(false);
            var rawContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return new ServiceResponse<T>
                               {
                                   Data = JsonConvert.DeserializeObject<T>(rawContent),
                                   Headers = GetResponseHeaders(response)
                               };
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

        public async Task<ServiceResponse<byte[]>> GetFileToDownload(HttpRequestMessage request)
        {
            request.RequestUri = ApplyAdditionalUrlMapping(request.RequestUri);
            var response = await this.httpClient.SendAsync(request);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            return new ServiceResponse<byte[]>
                       {
                           Data = bytes,
                           Headers = GetResponseHeaders(response)
                       };
        }

        public Dictionary<string, string> GetResponseHeaders(HttpResponseMessage message)
        {
            var headers = message.Headers.ToDictionary(k => k.Key, v => v.Value.Last());

            foreach (var httpContentHeader in message.Content.Headers)
            {
                headers.Add(httpContentHeader.Key, httpContentHeader.Value.Last());
            }

            return headers;
        }

        private Uri ApplyAdditionalUrlMapping(Uri requestUri)
        {
            var url = requestUri.ToString();
            url = url.Replace("[", "%5B")
                     .Replace("]", "%5D");
            return new Uri(url);
        }
    }
}
