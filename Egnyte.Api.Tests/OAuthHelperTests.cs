namespace Egnyte.Api.Tests
{
    using System;
    using System.Linq;

    using NUnit.Framework;

    [TestFixture]
    public class OAuthHelperTests
    {
        [Test]
        public void GetAuthorizeUri_ThrowsArgumentNullException_WhenUserDomainIsEmpty()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => OAuthHelper.GetAuthorizeUri(
                    OAuthAuthorizationFlow.Code,
                    null,
                    "id",
                    new Uri("https://myapp.com"),
                     "secret"));

            Assert.IsTrue(exception.Message.Contains("userDomain"));
        }

        [Test]
        public void GetAuthorizeUri_ThrowsArgumentNullException_WhenClientIdIsEmpty()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => OAuthHelper.GetAuthorizeUri(
                    OAuthAuthorizationFlow.Code,
                    "domain",
                    null,
                    new Uri("https://myapp.com"),
                    "secret"));

            Assert.IsTrue(exception.Message.Contains("clientId"));
        }

        [Test]
        public void GetAuthorizeUri_ThrowsArgumentNullException_WhenRedirectUriIsEmpty()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => OAuthHelper.GetAuthorizeUri(
                    OAuthAuthorizationFlow.Code,
                    "domain",
                    "id",
                    null,
                    "secret"));

            Assert.IsTrue(exception.Message.Contains("redirectUri"));
        }

        [Test]
        public void GetAuthorizeUri_ReturnsCorrectUri()
        {
            var uri = OAuthHelper.GetAuthorizeUri(
                OAuthAuthorizationFlow.Code,
                "acme",
                "Client123",
                new Uri("https://myapp.com/oauth"),
                "MyOwnSecret",
                "Egnyte.filesystem",
                "apidemo123",
                true);
            const string ExpectedUrl = "https://acme.egnyte.com/puboauth/token?client_id=Client123&redirect_uri=https://myapp.com/oauth&response_type=code&scope=Egnyte.filesystem&state=apidemo123&mobile=1";
            Assert.AreEqual(ExpectedUrl, uri.ToString());
        }

        [Test]
        public void GetAuthorizeUri_WithMinimumParametersSpecified_ReturnsCorrectUri()
        {
            var uri = OAuthHelper.GetAuthorizeUri(
                OAuthAuthorizationFlow.Code,
                "acme",
                "Client123",
                new Uri("https://myapp.com/oauth"),
                "MyOwnSecret");
            const string ExpectedUrl = "https://acme.egnyte.com/puboauth/token?client_id=Client123&redirect_uri=https://myapp.com/oauth&response_type=code";
            Assert.AreEqual(ExpectedUrl, uri.ToString());
        }

        [Test]
        public void GetAuthorizeUri_WithImplicitGrantAuthorizationFlow_ReturnsTokenAsResponseType()
        {
            var uri = OAuthHelper.GetAuthorizeUri(
                OAuthAuthorizationFlow.ImplicitGrant,
                "acme",
                "Client123",
                new Uri("https://myapp.com/oauth"));
            const string ExpectedUrl = "https://acme.egnyte.com/puboauth/token?client_id=Client123&redirect_uri=https://myapp.com/oauth&response_type=token";
            Assert.AreEqual(ExpectedUrl, uri.ToString());
        }

        [Test]
        public void GetAuthorizationUriResourceOwnerFlow_ReturnsUrl_WhenCorrectParameters()
        {
            var uri = OAuthHelper.GetAuthorizationUriResourceOwnerFlow(
                "acme",
                "Client123",
                "user123",
                "password123");

            Assert.AreEqual("https://acme.egnyte.com/puboauth/token", uri.BaseAddress.ToString());
            Assert.AreEqual("Client123", uri.QueryParameters["client_id"]);
            Assert.AreEqual("user123", uri.QueryParameters["username"]);
            Assert.AreEqual("password123", uri.QueryParameters["password"]);
            Assert.AreEqual("password", uri.QueryParameters["grant_type"]);
        }

        [Test]
        public void GetAuthorizationUriResourceOwnerFlow_WithAllParametersSpecified_ReturnsUrl_WhenCorrectParameters()
        {
            var uri = OAuthHelper.GetAuthorizationUriResourceOwnerFlow(
                "acme",
                "Client123",
                "user123",
                "password123",
                "8WkD6YhXJDZrV7kWABQtr2bXBUY5GRTmuqBpRs4JDWHkNNhSK9");

            Assert.AreEqual("https://acme.egnyte.com/puboauth/token", uri.BaseAddress.ToString());
            Assert.AreEqual("Client123", uri.QueryParameters["client_id"]);
            Assert.AreEqual("user123", uri.QueryParameters["username"]);
            Assert.AreEqual("password123", uri.QueryParameters["password"]);
            Assert.AreEqual("password", uri.QueryParameters["grant_type"]);
            Assert.AreEqual("8WkD6YhXJDZrV7kWABQtr2bXBUY5GRTmuqBpRs4JDWHkNNhSK9", uri.QueryParameters["client_secret"]);
        }

        [Test]
        public void GetAuthorizationUriResourceOwnerFlow_ThrowsArgumentNullException_WhenDomainIsEmpty()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                    () =>
                    OAuthHelper.GetAuthorizationUriResourceOwnerFlow(
                        string.Empty,
                        "Client123",
                        "user123",
                        "password123"));

            Assert.IsTrue(exception.Message.Contains("userDomain"));
        }

        [Test]
        public void GetAuthorizationUriResourceOwnerFlow_ThrowsArgumentNullException_WhenClientIdIsEmpty()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                    () =>
                    OAuthHelper.GetAuthorizationUriResourceOwnerFlow(
                        "acme",
                        string.Empty,
                        "user123",
                        "password123"));

            Assert.IsTrue(exception.Message.Contains("clientId"));
        }

        [Test]
        public void GetAuthorizationUriResourceOwnerFlow_ThrowsArgumentNullException_WhenUsernameIsEmpty()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                    () =>
                    OAuthHelper.GetAuthorizationUriResourceOwnerFlow(
                        "acme",
                        "Client123",
                        null,
                        "password123"));

            Assert.IsTrue(exception.Message.Contains("username"));
        }

        [Test]
        public void GetAuthorizationUriResourceOwnerFlow_ThrowsArgumentNullException_WhenPasswordIsEmpty()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                    () =>
                    OAuthHelper.GetAuthorizationUriResourceOwnerFlow(
                        "acme",
                        "Client123",
                        "Username",
                        string.Empty));

            Assert.IsTrue(exception.Message.Contains("password"));
        }

        [Test]
        public void GetTokenUri_ThrowsArgumentNullException_WhenUserDomainIsEmpty()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => OAuthHelper.GetTokenRequestParameters(
                    null,
                    "id",
                    "secret",
                    new Uri("https://myapp.com"),
                    "code"));

            Assert.IsTrue(exception.Message.Contains("userDomain"));
        }

        [Test]
        public void GetTokenUri_ThrowsArgumentNullException_WhenClientIdIsEmpty()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => OAuthHelper.GetTokenRequestParameters(
                    "domain",
                    null,
                    "secret",
                    new Uri("https://myapp.com"),
                    "code"));

            Assert.IsTrue(exception.Message.Contains("clientId"));
        }

        [Test]
        public void GetTokenUri_ThrowsArgumentNullException_WhenClientSecretIsEmpty()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => OAuthHelper.GetTokenRequestParameters(
                    "domain",
                    "id",
                    null,
                    new Uri("https://myapp.com"),
                    "code"));

            Assert.IsTrue(exception.Message.Contains("clientSecret"));
        }

        [Test]
        public void GetTokenUri_ThrowsArgumentNullException_WhenRedirectUriIsEmpty()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => OAuthHelper.GetTokenRequestParameters(
                    "domain",
                    "id",
                    "secret",
                    null,
                    "code"));

            Assert.IsTrue(exception.Message.Contains("redirectUri"));
        }

        [Test]
        public void GetTokenUri_ThrowsArgumentNullException_WhenAuthorizationCodeIsEmpty()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => OAuthHelper.GetTokenRequestParameters(
                    "domain",
                    "id",
                    "secret",
                    new Uri("https://myapp.com"),
                    null));

            Assert.IsTrue(exception.Message.Contains("authorizationCode"));
        }

        [Test]
        public void GetTokenUri_ReturnsCorrectUri()
        {
            var uri = OAuthHelper.GetTokenRequestParameters(
                "acme",
                "Client123",
                "MyOwnSecret",
                new Uri("https://myapp.com/oauth"),
                "code");
            const string ExpectedBaseUrl = "https://acme.egnyte.com/puboauth/token";
            const string ExpectedQueryParameters = "client_id=Client123&client_secret=MyOwnSecret&redirect_uri=https://myapp.com/oauth&code=code&grant_type=authorization_code";

            var uriQueryParametersSerialized = string.Join("&", uri.QueryParameters.Select(q => q.Key + "=" + q.Value));

            Assert.AreEqual(ExpectedBaseUrl, uri.BaseAddress.ToString());
            Assert.AreEqual(ExpectedQueryParameters, uriQueryParametersSerialized);
        }

        [Test]
        public void ParseTokenFromUrlResponse_ReturnsToken_WhenCorrectUrl()
        {
            var token = OAuthHelper.ParseTokenFromUrlResponse("#access_token=OAUTH_CODE&token_type=bearer&state=apidemo123");

            Assert.AreEqual("OAUTH_CODE", token.AccessToken);
            Assert.AreEqual("bearer", token.TokenType);
            Assert.AreEqual("apidemo123", token.State);
            Assert.AreEqual(string.Empty, token.Error);
            Assert.AreEqual(string.Empty, token.ExpiresIn);
        }

        [Test]
        public void ParseTokenFromUrlResponse_ReturnsCorrectErrorResponse_WhenAuthorizationFailed()
        {
            var token = OAuthHelper.ParseTokenFromUrlResponse("#error=access_denied&state=apidemo123");

            Assert.AreEqual("access_denied", token.Error);
            Assert.AreEqual("apidemo123", token.State);
            Assert.AreEqual(string.Empty, token.AccessToken);
            Assert.AreEqual(string.Empty, token.TokenType);
            Assert.AreEqual(string.Empty, token.ExpiresIn);
        }
    }
}
