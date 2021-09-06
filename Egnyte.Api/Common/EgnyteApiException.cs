using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Egnyte.Api.Common
{
    using System.Net;

    public class EgnyteApiException : Exception
    {
        public EgnyteApiException(string message, HttpResponseMessage response, Exception innerException = null)
            : base(message, innerException)
        {
            StatusCode = response.StatusCode;
            Headers = response.GetResponseHeaders();
        }

        public EgnyteApiException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Status code of response from Egnyte API
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// Headers from response
        /// </summary>
        public IDictionary<string, string> Headers { get; private set; }
    }
}
