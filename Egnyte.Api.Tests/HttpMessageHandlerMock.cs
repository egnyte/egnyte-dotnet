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

        private string content;

        public void SetException(Exception exceptionArg)
        {
            exception = exceptionArg;
        }

        public HttpRequestMessage GetHttpRequestMessage()
        {
            return requestMessage;
        }

        public string GetRequestContentAsString()
        {
            return content;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            try
            {
                content = request.Content.ReadAsStringAsync().Result;
            }
            catch (Exception) {}
            
            requestMessage = request;

            if (this.exception != null)
            {
                throw this.exception;
            }

            return SendAsyncFunc(request, cancellationToken);
        }
    }
}
