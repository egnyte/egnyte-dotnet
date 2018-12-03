using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Egnyte.Api.Common
{
    public static class HttpResponseMessageExtensions
    {
        public static Dictionary<string, string> GetResponseHeaders(this HttpResponseMessage message)
        {
            if (message == null || message.Headers == null || message.Content.Headers == null)
            {
                return new Dictionary<string, string>();
            }

            var headers = message.Headers.ToDictionary(k => k.Key, v => v.Value.Last());

            foreach (var httpContentHeader in message.Content.Headers)
            {
                headers.Add(httpContentHeader.Key, httpContentHeader.Value.Last());
            }

            return headers;
        }
    }
}
