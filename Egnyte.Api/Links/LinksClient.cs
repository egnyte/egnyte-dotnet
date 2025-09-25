using Egnyte.Api.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Egnyte.Api.Links
{
    public class LinksClient : BaseClient
    {
        const string LinkMethod = "/pubapi/v1/links";
        const string LinkMethodV2 = "/pubapi/v2/links";

        internal LinksClient(HttpClient httpClient, string domain = "", string host = "") : base(httpClient, domain, host) { }

        /// <summary>
        /// Lists all links. Please note, that if the user executing this method is not an admin,
        /// then only links created by the user will be listed
        /// </summary>
        /// <param name="path">Optional. List links to a file or folder specified by its full path</param>
        /// <param name="userName">Optional. List links created by this user</param>
        /// <param name="createdBefore">Optional. List links created before a given date</param>
        /// <param name="createdAfter">Optional. List links created after a given date</param>
        /// <param name="linkType">Optional. List links that are "file" or "folder"</param>
        /// <param name="accessibility">Optional. Filter to links whose accessiblity is "anyone,"
        /// "password," "domain," or "recipients"</param>
        /// <param name="offset">Optional. The 0-based index of the initial record being requested</param>
        /// <param name="count">Optional. Limit number of entries per page. By default,
        /// all entries are returned</param>
        /// <returns></returns>
        public async Task<LinksList> ListLinks(
            string path = null,
            string userName = null,
            DateTime? createdBefore = null,
            DateTime? createdAfter = null,
            LinkType? linkType = null,
            LinkAccessibility? accessibility = null,
            int? offset = null,
            int? count = null)
        {
            var httpRequest = new HttpRequestMessage(
                HttpMethod.Get,
                ListLinksRequestUri(
                    LinkMethod,
                    path,
                    userName,
                    createdBefore,
                    createdAfter,
                    linkType,
                    accessibility,
                    offset,
                    count));

            var serviceHandler = new ServiceHandler<LinksList>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return response.Data;
        }

        /// <summary>
        /// Lists all links with details. Please note, that if the user executing this method is not an admin,
        /// then only links created by the user will be listed
        /// </summary>
        /// <param name="path">Optional. List links to a file or folder specified by its full path</param>
        /// <param name="userName">Optional. List links created by this user</param>
        /// <param name="createdBefore">Optional. List links created before a given date
        /// (ISO-8601 e.g., 2017-03-05T14:55:59+0000).</param>
        /// <param name="createdAfter">Optional. List links created after a given date
        /// (ISO-8601 e.g., 2017-03-05T14:55:59+0000).</param>
        /// <param name="linkType">Optional. List links that are "file" or "folder"</param>
        /// <param name="accessibility">Optional. Filter to links whose accessiblity is "anyone,"
        /// "password," "domain," or "recipients"</param>
        /// <param name="offset">Optional. The 0-based index of the initial record being requested</param>
        /// <param name="count">Limit number of entries per page.
        /// By default the max count of 500 entries is returned.</param>
        /// <returns></returns>
        public async Task<LinksListV2> ListLinksV2(
            string path = null,
            string userName = null,
            DateTime? createdBefore = null,
            DateTime? createdAfter = null,
            LinkType? linkType = null,
            LinkAccessibility? accessibility = null,
            int? offset = null,
            int? count = null)
        {
            var httpRequest = new HttpRequestMessage(
                HttpMethod.Get,
                ListLinksRequestUri(
                    LinkMethodV2,
                    path,
                    userName,
                    createdBefore,
                    createdAfter,
                    linkType,
                    accessibility,
                    offset,
                    count));

            var serviceHandler = new ServiceHandler<LinksListV2Response>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return LinksHelper.MapLinksListResponse(response.Data);
        }

        /// <summary>
        /// Gets the details of a link
        /// </summary>
        /// <param name="linkId">Required. Link id, retrieved earlier from Egnyte</param>
        /// <returns>Details of the link</returns>
        public async Task<LinkDetails> GetLinkDetails(string linkId)
        {
            if (string.IsNullOrWhiteSpace(linkId))
            {
                throw new ArgumentNullException(nameof(linkId));
            }

            var uriBuilder = BuildUri(LinkMethod + "/" + linkId);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

            var serviceHandler = new ServiceHandler<LinkDetailsResponse>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return LinksHelper.MapGetLinkDetailsResponse(response.Data);
        }

        /// <summary>
        /// Creates link
        /// </summary>
        /// <param name="link">Parameters</param>
        /// <returns>Created link details</returns>
        public async Task<CreatedLink> CreateLink(NewLink link)
        {
            ThrowExceptionsIfNewLinkIsInvalid(link);

            if (!link.Path.StartsWith("/", StringComparison.Ordinal))
            {
                link.Path = "/" + link.Path;
            }

            var uriBuilder = BuildUri(LinkMethod);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
            {
                Content = new StringContent(MapLinkForRequest(link), Encoding.UTF8, "application/json")
            };

            var serviceHandler = new ServiceHandler<CreatedLinkResponse>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return LinksHelper.MapFlatCreatedLinkToCreatedLink(response.Data);
        }

        /// <summary>
        /// Delets a link
        /// </summary>
        /// <param name="linkId">Required. Link id, retrieved earlier from Egnyte</param>
        /// <returns>Returns true if deleting link succeeded</returns>
        public async Task<bool> DeleteLink(string linkId)
        {
            if (string.IsNullOrWhiteSpace(linkId))
            {
                throw new ArgumentNullException(nameof(linkId));
            }

            var uriBuilder = BuildUri(LinkMethod + "/" + linkId);
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, uriBuilder.Uri);

            var serviceHandler = new ServiceHandler<string>(httpClient);
            await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return true;
        }

        string MapLinkForRequest(NewLink link)
        {
            var builder = new StringBuilder();
            builder
                .Append("{")
                .Append("\"path\" : \"" + link.Path + "\",")
                .Append("\"type\" : \"" + MapLinkType(link.Type) + "\"" );

            if (link.Type != LinkType.Upload)
                builder.AppendFormat(@", ""accessibility"": ""{0}""", MapAccessibilityType(link.Accessibility));            

            if (!string.IsNullOrWhiteSpace(link.Password))
            {
                builder.Append(", \"password\": \"" + link.Password + "\"");
            }

            if (link.SendEmail.HasValue)
            {
                builder.AppendFormat(@", ""send_email"" : ""{0}""", link.SendEmail.Value ? "true" : "false");
                if (link.Recipients.Count > 0)
                {
                    builder.Append(", \"recipients\": [");
                    builder.Append(string.Join(", ", link.Recipients.Select(r => "\"" + r + "\"")));
                    builder.Append("]");
                }
            }
   
            if (!string.IsNullOrWhiteSpace(link.Message))
            {
                builder.Append(", \"message\": \"" + link.Message + "\"");
            }

            if (link.CopyMe.HasValue)
            {
                builder.AppendFormat(@", ""copy_me"": ""{0}""", link.CopyMe.Value ? "true" : "false");
            }

            if (link.Notify.HasValue)
            {
                builder.AppendFormat(@", ""notify"": ""{0}""", link.Notify.Value ? "true" : "false");
            }

            if (link.LinkToCurrent.HasValue)
            {
                builder.AppendFormat(@", ""link_to_current"": ""{0}""", link.LinkToCurrent.Value ? "true" : "false");
            }

            if (link.ExpiryDate.HasValue)
            {
                builder.AppendFormat(@", ""expiry_date"": ""{0}""", link.ExpiryDate.Value.ToString("yyyy-MM-dd"));
            }

            if (link.ExpiryClicks.HasValue)
            {
                builder.Append(", \"expiry_clicks\": \"" + link.ExpiryClicks.Value + "\"");
            }

            if (link.FolderPerRecipient.HasValue)
            {
                builder.AppendFormat(@", ""folder_per_recipient"": ""{0}""", link.FolderPerRecipient.Value ? "true" : "false");
            }

            builder.Append("}");

            return builder.ToString();
        }

        void ThrowExceptionsIfNewLinkIsInvalid(NewLink link)
        {
            if (link == null)
            {
                throw new ArgumentNullException(nameof(link));
            }

            if (string.IsNullOrWhiteSpace(link.Path))
            {
                throw new ArgumentNullException(nameof(link.Path));
            }
        }

        

        Uri ListLinksRequestUri(
            string linkMethod,
            string path,
            string userName,
            DateTime? createdBefore,
            DateTime? createdAfter,
            LinkType? linkType,
            LinkAccessibility? accessibility,
            int? offset,
            int? count)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(path))
            {
                queryParams.Add("path=" + EncodeQueryPath(path));
            }

            if (!string.IsNullOrWhiteSpace(userName))
            {
                queryParams.Add("username=" + userName);
            }

            if (createdBefore.HasValue)
            {
                queryParams.Add("created_before=" + createdBefore.Value.ToString("yyyy-MM-dd"));
            }

            if (createdAfter.HasValue)
            {
                queryParams.Add("created_after=" + createdAfter.Value.ToString("yyyy-MM-dd"));
            }

            if (linkType.HasValue)
            {
                queryParams.Add("type=" + MapLinkType(linkType.Value));
            }

            if (accessibility.HasValue)
            {
                queryParams.Add("accessibility=" + MapAccessibilityType(accessibility.Value));
            }

            if (offset.HasValue)
            {
                queryParams.Add("offset=" + offset.Value);
            }

            if (count.HasValue)
            {
                queryParams.Add("count=" + count);
            }

            var query = string.Join("&", queryParams);

            var uriBuilder = BuildUri(linkMethod, query);

            return uriBuilder.Uri;
        }

        string MapAccessibilityType(LinkAccessibility value)
        {
            switch (value)
            {
                case LinkAccessibility.Domain:
                    return "domain";
                case LinkAccessibility.Password:
                    return "password";
                case LinkAccessibility.Recipients:
                    return "recipients";
                default:
                    return "anyone";
            }
        }

        string MapLinkType(LinkType linkType)
        {
            switch (linkType)
            {
                case LinkType.File: return "file";
                case LinkType.Upload: return "upload";                
                default: return "folder";
            }
        }
    }
}
