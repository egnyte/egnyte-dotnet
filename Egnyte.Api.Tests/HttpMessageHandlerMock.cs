namespace Egnyte.Api.Tests
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class HttpMessageHandlerMock : HttpClientHandler
    {
        public Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> SendAsyncFunc { get; set; }

        private Exception exception;

        private HttpRequestMessage requestMessage;

        public void SetException(Exception exceptionArg)
        {
            exception = exceptionArg;
        }

        public HttpRequestMessage GetHttpRequestMessage()
        {
            return requestMessage;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            requestMessage = request;

            if (this.exception != null)
            {
                throw this.exception;
            }

            return SendAsyncFunc(request, cancellationToken);
        }
    }
}
