﻿using Egnyte.Api.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Egnyte.Api.Groups
{
    public class GroupsClient : BaseClient
    {
        const string GroupMethod = "/pubapi/v2/groups";

        internal GroupsClient(HttpClient httpClient, string domain = "", string host = "") : base(httpClient, domain, host) { }

        /// <summary>
        /// Lists user groups
        /// </summary>
        /// <param name="startIndex">Optional. The 1-based index of the initial record
        /// being requested (Integer ≥ 1)</param>
        /// <param name="count">Optional. The number of entries per page (min 1, max 100)</param>
        /// <param name="filter">Allows you to request a subset of groups
        /// These terms are not case sensitive. For example:
        /// filter=displayname sw "acc" (for starts with "acc")</param>
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

            var uriBuilder = BuildUri(GroupMethod, GetListGroupsRequestQuery(startIndex, count, filter));
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

            var serviceHandler = new ServiceHandler<Groups>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return response.Data;
        }

        /// <summary>
        /// Shows which users are in the group and view group attributes
        /// </summary>
        /// <param name="groupId">Required. The globally unique group ID</param>
        /// <returns>Group details and it's members</returns>
        public async Task<GroupDetails> ShowSingleGroup(string groupId)
        {
            if (string.IsNullOrWhiteSpace(groupId))
            {
                throw new ArgumentNullException(nameof(groupId));
            }
            
            var uriBuilder = BuildUri(GroupMethod + "/" + groupId);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

            var serviceHandler = new ServiceHandler<GroupDetails>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return response.Data;
        }

        /// <summary>
        /// Creates user group
        /// </summary>
        /// <param name="displayName">Required. The name of the group</param>
        /// <param name="members">Required. An array containing all users in the group</param>
        /// <returns>Group details and it's members</returns>
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

            var uriBuilder = BuildUri(GroupMethod);
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
        /// a change to settings that ensures all prior settings are removed
        /// </summary>
        /// <param name="groupId">Required. The globally unique group ID</param>
        /// <param name="displayName">Required. The name of the group</param>
        /// <param name="members">Required. An array containing all users in the group</param>
        /// <returns>Group details and it's members</returns>
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

            var uriBuilder = BuildUri(GroupMethod + "/" + groupId);
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

        /// <summary>
        /// Updates specific attributes of a group. This is especially useful for making
        /// incremental modifications to a folder
        /// </summary>
        /// <param name="groupId">Required. The globally unique group ID</param>
        /// <param name="displayName">Optional. Specifying this parameter will rename the group</param>
        /// <param name="members">Optional. An array containing all users being modified.
        /// It will modify only users specifyied. If you want to remove a user,
        /// use DeleteUser property in GroupMember class.</param>
        /// <returns>Group details and it's members</returns>
        public async Task<GroupDetails> PartialGroupUpdate(
            string groupId,
            string displayName = null,
            List<GroupMember> members = null)
        {
            if (string.IsNullOrWhiteSpace(groupId))
            {
                throw new ArgumentNullException(nameof(groupId));
            }

            var uriBuilder = BuildUri(GroupMethod + "/" + groupId);
            var httpRequest = new HttpRequestMessage(new HttpMethod("PATCH"), uriBuilder.Uri)
            {
                Content = new StringContent(
                    GetPartialGroupUpdateContent(displayName, members),
                    Encoding.UTF8,
                    "application/json")
            };

            var serviceHandler = new ServiceHandler<GroupDetails>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return response.Data;
        }

        /// <summary>
        /// Deletes a group
        /// </summary>
        /// <param name="groupId">Required. The globally unique group ID</param>
        /// <returns>True if deletion succeeded</returns>
        public async Task<bool> DeleteGroup(string groupId)
        {
            if (string.IsNullOrWhiteSpace(groupId))
            {
                throw new ArgumentNullException(nameof(groupId));
            }

            var uriBuilder = BuildUri(GroupMethod + "/" + groupId);
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, uriBuilder.Uri);

            var serviceHandler = new ServiceHandler<string>(httpClient);
            await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return true;
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

        string GetPartialGroupUpdateContent(string displayName, List<GroupMember> members)
        {
            var membersStringified = new List<string>();

            if (members != null && members.Count > 0)
            {
                foreach (var member in members)
                {
                    var singleMember = "{";
                    if (member.DeleteUser)
                    {
                        singleMember += "\"operation\":\"delete\",";
                    }

                    singleMember += "\"value\":" + member.Id;
                    singleMember += "}";
                    membersStringified.Add(singleMember);
                }
            }
            
            var builder = new StringBuilder();
            builder.Append("{");

            if (!string.IsNullOrWhiteSpace(displayName))
            {
                builder.Append("\"displayName\": \"" + displayName + "\"");
            }
            
            if (membersStringified.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(displayName))
                {
                    builder.Append(", ");
                }

                builder.Append("\"members\":[" + string.Join(",", membersStringified) + "]");
            }

            builder.Append("}");
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
