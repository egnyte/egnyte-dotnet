using System;
using System.Net.Http;

namespace Egnyte.Api.Common
{
    public static class ExceptionHelper
    {
        public static void CheckErrorStatusCode(HttpResponseMessage response, string responseContent = "")
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }
            
            var headers = response.GetLowercaseResponseHeaders();
            if (headers.ContainsKey("x-mashery-error-code"))
            {
                if (headers["x-mashery-error-code"] == "ERR_403_DEVELOPER_OVER_QPS")
                {
                    throw new QPSLimitExceededException(headers);
                }

                if (headers["x-mashery-error-code"] == "ERR_403_DEVELOPER_OVER_RATE")
                {
                    throw new RateLimitExceededException(headers);
                }
            }

            throw new EgnyteApiException(responseContent, response);
        }
    }
}
