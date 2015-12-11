using System;
using System.Text;

namespace Egnyte.Api
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    public enum OAuthAuthorizationFlow
    {
        /// <summary>
        /// This represents OAuth 2.0 Authorization Code flow.
        /// The authorization code flow is most commonly used by applications that make server side requests.
        /// </summary>
        Code,

        /// <summary>
        /// This represents OAuth 2.0 Implicit Grant flow.
        /// The implicit grant flow is most commonly used by applications browser applications.
        /// </summary>
        ImplicitGrant
    }

    public static class OAuthHelper
    {
        private const string EgnyteBaseUrl = "https://{0}.egnyte.com/puboauth/token";

        /// <summary>
        /// Gets the URI used to initiate the OAuth2.0 authorization flow.
        /// </summary>
        /// <param name="oAuthFlow">This is the OAuth 2.0 flow to authenticate your app.
        /// More information about flow can be find 
        /// <a href="https://developers.egnyte.com/docs/read/Public_API_Authentication#Public-Applications">documentation</a>
        /// </param>
        /// <param name="userDomain">This is the user's Egnyte domain. An example domain name is "acme".
        /// Note that initially your API key will only work with the single Egnyte domain you told us you would use for testing.
        /// When we approve your completed app, we will issue you a new key that works with any Egnyte domain.</param>
        /// <param name="clientId">This is the API key that was provided to you when you registered your application.</param>
        /// <param name="clientSecret">This is the secret key that was provided with your key to you when you registered
        /// your application. If your application key was requested prior to January 2015, please register for a new key
        /// to get one with a client secret.</param>
        /// <param name="redirectUri">This is the URL that we will redirect to after the user has allowed or denied
        /// your request to access their account. This must be an HTTPS URL and must match the callback URL configured
        /// for your key. E.g. "https://yourapp.com/oauth"</param>
        /// <param name="scope">Although this parameter is optional from a technical standpoint, all third party applications
        /// must scope token requests to only the endpoints they need. We will not approve applications for production use
        /// unless they properly scope their token requests or demonstrate a valid need for global scope. More details
        /// on OAuth Scopes are provided
        /// <a href="https://developers.egnyte.com/docs/read/Public_API_Authentication#oauth_scope">here</a>.</param>
        /// <param name="state">As described in the OAuth 2.0 spec, this optional parameter is an opaque value used by the client
        /// to maintain state between the request and callback. The authorization server includes this value when redirecting
        /// the user-agent back to the client. The parameter can be used for preventing cross-site request forgery and passing
        /// the Egnyte domain as part of the response from the authorization server.</param>
        /// <param name="mobile">You may optionally set this parameter to true to have the user directed to a mobile friendly
        /// login page instead of the default page designed for a desktop browser.</param>
        /// <returns>The url, where the user will be taken to an authorization page that asks them to explicitly permit your app
        /// to access their Egnyte account. This page displays information about your application taken from your profile.
        /// The user must provide their login credentials on this page to authorize your application.</returns>
        public static Uri GetAuthorizeUri(
            OAuthAuthorizationFlow oAuthFlow,
            string userDomain,
            string clientId,
            string clientSecret,
            Uri redirectUri,
            string scope = null,
            string state = null,
            bool? mobile = null)
        {
            if (string.IsNullOrWhiteSpace(userDomain))
            {
                throw new ArgumentNullException("userDomain");
            }

            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentNullException("clientId");
            }

            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                throw new ArgumentNullException("clientSecret");
            }

            if (redirectUri == null || string.IsNullOrWhiteSpace(Uri.EscapeDataString(redirectUri.ToString())))
            {
                throw new ArgumentNullException("redirectUri");
            }

            var queryBuilder = new StringBuilder();
            queryBuilder.Append("client_id=").Append(clientId);
            queryBuilder.Append("&client_secret=").Append(clientSecret);
            queryBuilder.Append("&redirect_uri=").Append(redirectUri);
            queryBuilder.Append("&response_type=").Append(oAuthFlow == OAuthAuthorizationFlow.Code ? "code" : "token");

            if (!string.IsNullOrWhiteSpace(scope))
            {
                queryBuilder.Append("&scope=").Append(scope);
            }

            if (!string.IsNullOrWhiteSpace(state))
            {
                queryBuilder.Append("&state=").Append(state);
            }

            if (mobile != null)
            {
                queryBuilder.Append("&mobile=").Append(mobile.Value ? "1" : "0");
            }

            var baseUri = new Uri(string.Format(EgnyteBaseUrl, userDomain));
            var uriBuilder = new UriBuilder(baseUri) { Query = queryBuilder.ToString() };

            return uriBuilder.Uri;
        }

        /// <summary>
        /// Send request to exchange the code for a token
        /// </summary>
        /// <param name="userDomain">This is the user's Egnyte domain. An example domain name is "acme".
        /// Note that initially your API key will only work with the single Egnyte domain you told us you would use for testing.
        /// When we approve your completed app, we will issue you a new key that works with any Egnyte domain.</param>
        /// <param name="clientId">This is the API key that was provided to you when you registered your application.</param>
        /// <param name="clientSecret">This is the secret key that was provided with your key to you when you registered
        /// your application. If your application key was requested prior to January 2015, please register for a new key
        /// to get one with a client secret.</param>
        /// <param name="redirectUri">This is the URL that we will redirect to after the user has allowed or denied
        /// your request to access their account. This must be an HTTPS URL and must match the callback URL configured
        /// for your key. E.g. "https://yourapp.com/oauth"</param>
        /// <param name="authorizationCode">The authorization code received from the authorization server after allowing
        /// application to use Egnyte account.</param>
        /// <param name="httpClient">Http client for sending the request</param>
        /// <returns>Token info</returns>
        public static async Task<TokenResponse> GetTokenFromCode(
            string userDomain,
            string clientId,
            string clientSecret,
            Uri redirectUri,
            string authorizationCode,
            HttpClient httpClient = null)
        {
            httpClient = httpClient ?? new HttpClient();
            var requestParameters = GetTokenRequestParameters(userDomain, clientId, clientSecret, redirectUri, authorizationCode);
            var content = new FormUrlEncodedContent(requestParameters.QueryParameters);
            var result = await httpClient.PostAsync(requestParameters.BaseAddress, content).ConfigureAwait(false);

            var rawContent = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonConvert.DeserializeObject<TokenResponse>(rawContent);
        }

        /// <summary>
        /// Gets the URI used for exchanging the code for a token
        /// </summary>
        /// <param name="userDomain">This is the user's Egnyte domain. An example domain name is "acme".
        /// Note that initially your API key will only work with the single Egnyte domain you told us you would use for testing.
        /// When we approve your completed app, we will issue you a new key that works with any Egnyte domain.</param>
        /// <param name="clientId">This is the API key that was provided to you when you registered your application.</param>
        /// <param name="clientSecret">This is the secret key that was provided with your key to you when you registered
        /// your application. If your application key was requested prior to January 2015, please register for a new key
        /// to get one with a client secret.</param>
        /// <param name="redirectUri">This is the URL that we will redirect to after the user has allowed or denied
        /// your request to access their account. This must be an HTTPS URL and must match the callback URL configured
        /// for your key. E.g. "https://yourapp.com/oauth"</param>
        /// <param name="authorizationCode">The authorization code received from the authorization server after allowing
        /// application to use Egnyte account.</param>
        /// <returns>Uri used for exchanging the code for a token</returns>
        public static TokenRequestParameters GetTokenRequestParameters(
            string userDomain,
            string clientId,
            string clientSecret,
            Uri redirectUri,
            string authorizationCode)
        {
            if (string.IsNullOrWhiteSpace(userDomain))
            {
                throw new ArgumentNullException("userDomain");
            }

            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentNullException("clientId");
            }

            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                throw new ArgumentNullException("clientSecret");
            }

            if (redirectUri == null || string.IsNullOrWhiteSpace(Uri.EscapeDataString(redirectUri.ToString())))
            {
                throw new ArgumentNullException("redirectUri");
            }

            if (authorizationCode == null || string.IsNullOrWhiteSpace(authorizationCode))
            {
                throw new ArgumentNullException("authorizationCode");
            }

            var queryParameters = new Dictionary<string, string>();
            queryParameters.Add("client_id", clientId);
            queryParameters.Add("client_secret", clientSecret);
            queryParameters.Add("redirect_uri", redirectUri.ToString());
            queryParameters.Add("code", authorizationCode);
            queryParameters.Add("grant_type", "authorization_code");

            return new TokenRequestParameters
                {
                    BaseAddress = new Uri(string.Format(EgnyteBaseUrl, userDomain)),
                    QueryParameters = queryParameters
                };
        }
    }
}
