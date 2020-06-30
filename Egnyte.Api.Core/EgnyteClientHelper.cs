namespace Egnyte.Api
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    public static class EgnyteClientHelper
    {
        /// <summary>
        /// Send request to exchange the code for a token
        /// </summary>
        /// <param name="userDomain">This is the user's Egnyte domain. An example domain name is "acme".
        /// Note that initially your API key will only work with the single Egnyte domain you told us you would use for testing.
        /// When we approve your completed app, we will issue you a new key that works with any Egnyte domain</param>
        /// <param name="clientId">This is the API key that was provided to you when you registered your application</param>
        /// <param name="clientSecret">This is the secret key that was provided with your key to you when you registered
        /// your application. If your application key was requested prior to January 2015, please register for a new key
        /// to get one with a client secret</param>
        /// <param name="redirectUri">This is the URL that we will redirect to after the user has allowed or denied
        /// your request to access their account. This must be an HTTPS URL and must match the callback URL configured
        /// for your key. E.g. "https://yourapp.com/oauth"</param>
        /// <param name="authorizationCode">The authorization code received from the authorization server after allowing
        /// application to use Egnyte account</param>
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
            var disposeClient = httpClient == null;
            try
            {
                httpClient = httpClient ?? new HttpClient();
                var requestParameters = OAuthHelper.GetTokenRequestParameters(
                    userDomain,
                    clientId,
                    clientSecret,
                    redirectUri,
                    authorizationCode);
                var content = new FormUrlEncodedContent(requestParameters.QueryParameters);
                var result = await httpClient.PostAsync(requestParameters.BaseAddress, content).ConfigureAwait(false);

                var rawContent = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonConvert.DeserializeObject<TokenResponse>(rawContent);
            }
            finally
            {
                if (disposeClient)
                {
                    httpClient.Dispose();
                }
            }
        }

        public static async Task<TokenResponse> GetTokenResourceOwnerFlow(
            string userDomain,
            string clientId,
            string username,
            string password,
            HttpClient httpClient = null)
        {
            var disposeClient = httpClient == null;
            try
            {
                httpClient = httpClient ?? new HttpClient();
                var tokenRequesrUri = OAuthHelper.GetAuthorizationUriResourceOwnerFlow(
                    userDomain,
                    clientId,
                    username,
                    password);

                var content = new FormUrlEncodedContent(tokenRequesrUri.QueryParameters);
                var result = await httpClient.PostAsync(tokenRequesrUri.BaseAddress, content).ConfigureAwait(false);

                var rawContent = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonConvert.DeserializeObject<TokenResponse>(rawContent);
            }
            finally
            {
                if (disposeClient)
                {
                    httpClient.Dispose();
                }
            }
        }
    }
}
