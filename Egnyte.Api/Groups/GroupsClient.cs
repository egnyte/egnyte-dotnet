using Egnyte.Api.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Egnyte.Api.Groups
{
    public class GroupsClient
    {
        readonly HttpClient httpClient;

        readonly string domain;

        const string LinkBasePath = "https://{0}.egnyte.com/pubapi/v2/groups";

        internal GroupsClient(HttpClient httpClient, string domain)
        {
            this.httpClient = httpClient;
            this.domain = domain;
        }

        /// <summary>
        /// Lists user groups
        /// </summary>
        /// <param name="startIndex">Optional. The 1-based index of the initial record
        /// being requested (Integer ≥ 1).</param>
        /// <param name="count">Optional. The number of entries per page (min 1, max 100).</param>
        /// <param name="filter">Allows you to request a subset of groups.
        /// These terms are not case sensitive. For example:
        /// filter=displayname sw "acc" (for starts with "acc").</param>
        /// <returns></returns>
        public async Task<Groups> ListGroups(
            int? startIndex = null,
            int? count = null,
            string filter = null)
        {
            if (startIndex.HasValue && startIndex < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            if (count.HasValue && (count < 0 || count > 100))
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            var uriBuilder = new UriBuilder(string.Format(LinkBasePath, domain))
            {
                Query = GetListGroupsRequestQuery(startIndex, count, filter)
            };
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

            var serviceHandler = new ServiceHandler<Groups>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return response.Data;
        }

        static string GetListGroupsRequestQuery(
            int? startIndex,
            int? count,
            string filter)
        {
            var queryParams = new List<string>();
            if (startIndex.HasValue)
            {
                queryParams.Add("startIndex=" + startIndex);
            }

            if (count.HasValue)
            {
                queryParams.Add("count=" + count);
            }

            if (!string.IsNullOrWhiteSpace(filter))
            {
                queryParams.Add("filter=" + filter);
            }

            return string.Join("&", queryParams);
        }
    }
}
