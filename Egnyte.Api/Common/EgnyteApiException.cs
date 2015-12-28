using System;

namespace Egnyte.Api.Common
{
    using System.Net;

    public class EgnyteApiException : Exception
    {
        public EgnyteApiException(string message, HttpStatusCode statusCode, Exception innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; private set; }
    }
}
