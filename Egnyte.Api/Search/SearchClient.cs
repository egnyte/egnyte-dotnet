using Egnyte.Api.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Egnyte.Api.Search
{
    public class SearchClient
    {
        readonly HttpClient httpClient;

        readonly string domain;

        const string SearchBasePath = "https://{0}.egnyte.com/pubapi/v1/search";

        internal SearchClient(HttpClient httpClient, string domain)
        {
            this.httpClient = httpClient;
            this.domain = domain;
        }

        /// <summary>
        /// Searching for files
        /// </summary>
        /// <param name="query">Required. The search string you want to find</param>
        /// <param name="offset">Optional. The 0-based index of the initial record
        /// being requested (Integer ≥ 0.</param>
        /// <param name="count">Optional. The number of entries per page (min 1, max 100)</param>
        /// <param name="folder">Optional. Limit the result set to only items contained
        /// in the specified folder</param>
        /// <param name="modifiedBefore">Optional. Limit to results modified before given date</param>
        /// <param name="modifiedAfter">Optional. Limit to results modified after given date</param>
        /// <returns>Search results</returns>
        public async Task<SearchResults> Search(
            string query,
            int? offset = null,
            int? count = null,
            string folder = null,
            DateTime? modifiedBefore = null,
            DateTime? modifiedAfter = null)
        {
            VerifySearchParameters(query, offset, count);

            var requestQuery = GetSearchQuery(query, offset, count, folder, modifiedBefore, modifiedAfter);
            var uriBuilder = new UriBuilder(string.Format(SearchBasePath, domain))
            {
                Query = requestQuery
            };
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

            var serviceHandler = new ServiceHandler<SearchResults>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return response.Data;
        }

        void VerifySearchParameters(string query, int? offset, int? count)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (offset.HasValue && offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset should be greater or equal to 0.");
            }

            if (count.HasValue && (count < 1 || count > 100))
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count shoule be between 1 and 100");
            }
        }

        string GetSearchQuery(
            string query,
            int? offset,
            int? count,
            string folder,
            DateTime? modifiedBefore,
            DateTime? modifiedAfter)
        {
            var queryParams = new List<string>();

            queryParams.Add("query=" + query);

            if (offset.HasValue)
            {
                queryParams.Add("offset=" + offset);
            }

            if (count.HasValue)
            {
                queryParams.Add("count=" + count);
            }

            if (!string.IsNullOrWhiteSpace(folder))
            {
                queryParams.Add("folder=" + folder);
            }

            if (modifiedBefore.HasValue)
            {
                queryParams.Add(string.Format("modified_before={0:yyyy-MM-ddTHH:mm:ssZ}", modifiedBefore));
            }

            if (modifiedAfter.HasValue)
            {
                queryParams.Add(string.Format("modified_after={0:yyyy-MM-ddTHH:mm:ssZ}", modifiedAfter));
            }

            return string.Join("&", queryParams);
        }
        
    }
}