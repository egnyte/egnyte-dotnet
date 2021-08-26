using System.Collections.Generic;
using System.Linq;

namespace Egnyte.Api.Common
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.IO;

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
            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            var rawContent = response.Content != null ? await response.Content.ReadAsStringAsync().ConfigureAwait(false) : null;

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    if (typeof(T) == typeof(string))
                    {
                        return new ServiceResponse<T>
                        {
                            Data = rawContent as T,
                            Headers = GetLowercaseResponseHeaders(response)
                        };
                    }

                    return new ServiceResponse<T>
                    {
                        Data = JsonConvert.DeserializeObject<T>(rawContent),
                        Headers = GetLowercaseResponseHeaders(response)
                    };
                }
                catch (Exception e)
                {
                    throw new EgnyteApiException(
                        rawContent,
                        response,
                        e);
                }
            }

            throw new EgnyteApiException(
                    rawContent,
                    response);
        }

        public async Task<ServiceResponse<byte[]>> GetFileToDownload(HttpRequestMessage request)
        {
            request.RequestUri = ApplyAdditionalUrlMapping(request.RequestUri);
            var response = await this.httpClient.SendAsync(request);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            return new ServiceResponse<byte[]>
            {
                Data = bytes,
                Headers = GetLowercaseResponseHeaders(response)
            };
        }

        public async Task<ServiceResponse<Stream>> GetFileToDownloadAsStream(HttpRequestMessage request)
        {
            request.RequestUri = ApplyAdditionalUrlMapping(request.RequestUri);
            var response = await this.httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            var stream = await response.Content.ReadAsStreamAsync();

            return new ServiceResponse<Stream>
            {
                Data = stream,
                Headers = GetLowercaseResponseHeaders(response)
            };
        }

        private Uri ApplyAdditionalUrlMapping(Uri requestUri)
        {
            var url = requestUri.ToString();
            url = url.Replace("[", "%5B")
                     .Replace("]", "%5D");
            return new Uri(url);
        }

        private Dictionary<string, string> GetLowercaseResponseHeaders(HttpResponseMessage response)
        {
            return response.GetResponseHeaders().ToDictionary(k => k.Key.ToLower(), v => v.Value);
        }
    }
}