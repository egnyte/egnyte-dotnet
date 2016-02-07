using Egnyte.Api.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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

        /// <summary>
        /// Shows which users are in the group and view group attributes.
        /// </summary>
        /// <param name="groupId">Required. The globally unique group ID.</param>
        /// <returns>Group details and it's members.</returns>
        public async Task<GroupDetails> ShowSingleGruop(string groupId)
        {
            if (string.IsNullOrWhiteSpace(groupId))
            {
                throw new ArgumentNullException(nameof(groupId));
            }
            
            var uriBuilder = new UriBuilder(string.Format(LinkBasePath, domain) + "/" + groupId);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

            var serviceHandler = new ServiceHandler<GroupDetails>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return response.Data;
        }

        /// <summary>
        /// Creates user group
        /// </summary>
        /// <param name="displayName">Required. The name of the group.</param>
        /// <param name="members">Required. An array containing all users in the group.</param>
        /// <returns>Group details and it's members.</returns>
        public async Task<GroupDetails> CreateGroup(
            string displayName,
            List<long> members)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentNullException(nameof(displayName));
            }

            if (members == null)
            {
                throw new ArgumentNullException(nameof(members));
            }

            var uriBuilder = new UriBuilder(string.Format(LinkBasePath, domain));
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
            {
                Content = new StringContent(
                    GetCreateGroupContent(displayName, members),
                    Encoding.UTF8,
                    "application/json")
            };

            var serviceHandler = new ServiceHandler<GroupDetails>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return response.Data;
        }

        /// <summary>
        /// Overrides all of the attributes of a group. This is especially useful for making
        /// a change to settings that ensures all prior settings are removed.
        /// </summary>
        /// <param name="groupId">Required. The globally unique group ID.</param>
        /// <param name="displayName">Required. The name of the group.</param>
        /// <param name="members">Required. An array containing all users in the group.</param>
        /// <returns>Group details and it's members.</returns>
        public async Task<GroupDetails> FullGroupUpdate(
            string groupId,
            string displayName,
            List<long> members)
        {
            if (string.IsNullOrWhiteSpace(groupId))
            {
                throw new ArgumentNullException(nameof(groupId));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentNullException(nameof(displayName));
            }

            if (members == null)
            {
                throw new ArgumentNullException(nameof(members));
            }

            var uriBuilder = new UriBuilder(string.Format(LinkBasePath, domain) + "/" + groupId);
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, uriBuilder.Uri)
            {
                Content = new StringContent(
                    GetCreateGroupContent(displayName, members),
                    Encoding.UTF8,
                    "application/json")
            };

            var serviceHandler = new ServiceHandler<GroupDetails>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return response.Data;
        }

        string GetCreateGroupContent(string displayName, List<long> members)
        {
            var membersContent = string.Join(",", members.Select(m => "{\"value\":" + m + "}").ToArray());

            var builder = new StringBuilder();
            builder
                .Append("{")
                .Append("\"displayName\": \"" + displayName + "\"")
                .Append(", \"members\":[" + membersContent + "]")
                .Append("}");
            return builder.ToString();
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
