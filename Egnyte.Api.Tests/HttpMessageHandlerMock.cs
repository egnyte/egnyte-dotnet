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

        public void SetException(Exception exceptionArg)
        {
            exception = exceptionArg;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (this.exception != null)
            {
                throw this.exception;
            }

            return SendAsyncFunc(request, cancellationToken);
        }
    }
}
