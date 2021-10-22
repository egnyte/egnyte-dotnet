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

            ExceptionHelper.CheckErrorStatusCode(response, rawContent);

            try
            {
                if (typeof(T) == typeof(string))
                {
                    return new ServiceResponse<T>
                    {
                        Data = rawContent as T,
                        Headers = response.GetLowercaseResponseHeaders()
                    };
                }

                return new ServiceResponse<T>
                {
                    Data = JsonConvert.DeserializeObject<T>(rawContent),
                    Headers = response.GetLowercaseResponseHeaders()
                };
            }
            catch (Exception e)
            {
                throw new EgnyteApiException(rawContent, response, e);
            }
        }

        public async Task<ServiceResponse<byte[]>> GetFileToDownload(HttpRequestMessage request)
        {
            request.RequestUri = ApplyAdditionalUrlMapping(request.RequestUri);
            var response = await this.httpClient.SendAsync(request);

            ExceptionHelper.CheckErrorStatusCode(response);

            var bytes = await response.Content.ReadAsByteArrayAsync();
            return new ServiceResponse<byte[]>
            {
                Data = bytes,
                Headers = response.GetLowercaseResponseHeaders()
            };
        }

        public async Task<ServiceResponse<Stream>> GetFileToDownloadAsStream(HttpRequestMessage request)
        {
            request.RequestUri = ApplyAdditionalUrlMapping(request.RequestUri);
            var response = await this.httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            ExceptionHelper.CheckErrorStatusCode(response);

            var stream = await response.Content.ReadAsStreamAsync();

            return new ServiceResponse<Stream>
            {
                Data = stream,
                Headers = response.GetLowercaseResponseHeaders()
            };
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