﻿namespace Egnyte.Api.Tests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using NUnit.Framework;

    [TestFixture]
    public class EgnyteClientHelperTests
    {
        [Test]
        public async Task GetTokenFromCode_ReturnsCorrectToken()
        {
            const string Token = "e8gdxt5ypf3pfn25a9kdzcac";
            const string TokenType = "bearer";
            const string TokenExpiresIn = "-1";

            const string TokenResponseText =
                @"{{
                    'access_token':'{0}',
                    'token_type':'{1}',
                    'expires_in': '{2}'
                }}";
            var tokenResponse = string.Format(TokenResponseText, Token, TokenType, TokenExpiresIn);

            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc = (request, cancellationToken) => Task.FromResult(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(tokenResponse)
                });

            var tokenResult = await EgnyteClientHelper.GetTokenFromCode(
                "acme",
                "Client123",
                "MyOwnSecret",
                new Uri("https://myapp.com/oauth"),
                "code",
                httpClient);

            Assert.NotNull(tokenResult);
            Assert.AreEqual(Token, tokenResult.AccessToken);
            Assert.AreEqual(TokenType, tokenResult.TokenType);
            Assert.AreEqual(TokenExpiresIn, tokenResult.ExpiresIn);
        }

        [Test]
        public async Task GetTokenResourceOwnerFlow_ReturnsCorrectToken()
        {
            const string Token = "e8gdxt5ypf3pfn25a9kdzcac";
            const string TokenType = "bearer";
            const string TokenExpiresIn = "-1";

            const string TokenResponseText =
                @"{{
                    'access_token':'{0}',
                    'token_type':'{1}',
                    'expires_in': '{2}'
                }}";
            var tokenResponse = string.Format(TokenResponseText, Token, TokenType, TokenExpiresIn);

            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc = (request, cancellationToken) => Task.FromResult(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(tokenResponse)
                });

            var tokenResult = await EgnyteClientHelper.GetTokenResourceOwnerFlow(
                "acme",
                "Client123",
                "username",
                "password",
                httpClient);

            Assert.NotNull(tokenResult);
            Assert.AreEqual(Token, tokenResult.AccessToken);
            Assert.AreEqual(TokenType, tokenResult.TokenType);
            Assert.AreEqual(TokenExpiresIn, tokenResult.ExpiresIn);
        }
    }
}
