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
                    "secret",
                    new Uri("https://myapp.com")));

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
                    "secret",
                    new Uri("https://myapp.com")));

            Assert.IsTrue(exception.Message.Contains("clientId"));
        }

        [Test]
        public void GetAuthorizeUri_ThrowsArgumentNullException_WhenClientSecretIsEmpty()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => OAuthHelper.GetAuthorizeUri(
                    OAuthAuthorizationFlow.Code,
                    "domain",
                    "id",
                    null,
                    new Uri("https://myapp.com")));

            Assert.IsTrue(exception.Message.Contains("clientSecret"));
        }

        [Test]
        public void GetAuthorizeUri_ThrowsArgumentNullException_WhenRedirectUriIsEmpty()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => OAuthHelper.GetAuthorizeUri(
                    OAuthAuthorizationFlow.Code,
                    "domain",
                    "id",
                    "secret",
                    null));

            Assert.IsTrue(exception.Message.Contains("redirectUri"));
        }

        [Test]
        public void GetAuthorizeUri_ReturnsCorrectUri()
        {
            var uri = OAuthHelper.GetAuthorizeUri(
                OAuthAuthorizationFlow.Code,
                "acme",
                "Client123",
                "MyOwnSecret",
                new Uri("https://myapp.com/oauth"),
                "Egnyte.filesystem",
                "apidemo123",
                true);
            const string ExpectedUri = "https://acme.egnyte.com/puboauth/token?client_id=Client123&client_secret=MyOwnSecret&redirect_uri=https://myapp.com/oauth&response_type=code&scope=Egnyte.filesystem&state=apidemo123&mobile=1";
            Assert.AreEqual(ExpectedUri, uri.ToString());
        }

        [Test]
        public void GetAuthorizeUri_WithMinimumParametersSpecified_ReturnsCorrectUri()
        {
            var uri = OAuthHelper.GetAuthorizeUri(
                OAuthAuthorizationFlow.Code,
                "acme",
                "Client123",
                "MyOwnSecret",
                new Uri("https://myapp.com/oauth"));
            const string ExpectedUri = "https://acme.egnyte.com/puboauth/token?client_id=Client123&client_secret=MyOwnSecret&redirect_uri=https://myapp.com/oauth&response_type=code";
            Assert.AreEqual(ExpectedUri, uri.ToString());
        }

        [Test]
        public void GetAuthorizeUri_WithImplicitGrantAuthorizationFlow_ReturnsTokenAsResponseType()
        {
            var uri = OAuthHelper.GetAuthorizeUri(
                OAuthAuthorizationFlow.ImplicitGrant,
                "acme",
                "Client123",
                "MyOwnSecret",
                new Uri("https://myapp.com/oauth"));
            const string ExpectedUri = "https://acme.egnyte.com/puboauth/token?client_id=Client123&client_secret=MyOwnSecret&redirect_uri=https://myapp.com/oauth&response_type=token";
            Assert.AreEqual(ExpectedUri, uri.ToString());
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
    }
}
