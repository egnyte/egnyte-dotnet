using Egnyte.Api.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Egnyte.Api.Permissions
{
    public class PermissionsClient
    {
        readonly HttpClient httpClient;

        readonly string domain;

        const string PermissionsBasePath = "https://{0}.egnyte.com/pubapi/v1/perms";

        internal PermissionsClient(HttpClient httpClient, string domain)
        {
            this.httpClient = httpClient;
            this.domain = domain;
        }

        /// <summary>
        /// Sets the effective permission level for specific users or groups on a given folder.
        /// </summary>
        /// <param name="path">Required. Full path to the folder</param>
        /// <param name="permission">Required. Type of permission: None, Viewer, Editor, Full, Owner.</param>
        /// <param name="users">List of usernames to set permissions for.
        /// At least one user or group must be specified.</param>
        /// <param name="groups">List of groupnames to set permissions for.
        /// At least one user or group must be specified.</param>
        /// <returns>True if operation succedes</returns>
        public async Task<bool> SetFolderPermissions(
            string path,
            PermissionType permission,
            List<string> users = null,
            List<string> groups = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if ((users == null || users.Count == 0) && (groups == null || groups.Count == 0))
            {
                throw new ArgumentException("One of parameters: " + nameof(users) + " or " + nameof(groups) + " must be not empty.");
            }

            var uriBuilder = new UriBuilder(string.Format(PermissionsBasePath, domain) + "/folder/" + path);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
            {
                Content = new StringContent(
                    GetSetFolderPermissionsContent(permission, users, groups),
                    Encoding.UTF8,
                    "application/json")
            };

            var serviceHandler = new ServiceHandler<string>(httpClient);
            await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Gets the permissions for a given folder.
        /// </summary>
        /// <param name="path">Required. Full path to the folder</param>
        /// <param name="users">Optional. List of usernames to report on.
        /// If neither users nor groups is set then permissions for all subjects are returned.</param>
        /// <param name="groups">Optional. List of groups to report on.
        /// If neither users nor groups is set then permissions for all subjects are returned.</param>
        /// <returns></returns>
        public async Task<FolderPermissions> GetFolderPermissions(
            string path,
            List<string> users = null,
            List<string> groups = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            var query = GetGetFolderPermissionsQuery(users, groups);
            var uriBuilder = new UriBuilder(string.Format(PermissionsBasePath, domain) + "/folder/" + path)
            {
                Query = query
            };
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

            var serviceHandler = new ServiceHandler<FolderPermissionsResponse>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return MapFolderPermissions(response.Data);
        }

        /// <summary>
        /// Gets the effective permissions for a user for a given folder.
        /// This effective permission takes into account both user and group permissions
        /// that apply to the given user, along with permission inheritance.
        /// </summary>
        /// <param name="username">Required. Egnyte username.</param>
        /// <param name="path">Required. Full path to the folder.</param>
        /// <returns>Effective user permissions for folder.</returns>
        public async Task<PermissionType> GetEffectivePermissionsForUser(
            string username,
            string path)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            
            if (!path.StartsWith("/", StringComparison.Ordinal))
            {
                path = "/" + path;
            }

            var uriBuilder = new UriBuilder(string.Format(PermissionsBasePath, domain) + "/user/" + username)
            {
                Query = "folder=" + path
            };
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

            var serviceHandler = new ServiceHandler<EffectivePermissionsResponse>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return ParsePermissionType(response.Data.Permission);
        }

        FolderPermissions MapFolderPermissions(FolderPermissionsResponse data)
        {
            return new FolderPermissions
            {
                Users = data.Users.Select(
                    u => new GroupOrUserPermissions(
                        u.Subject,
                        ParsePermissionType(u.Permission)))
                    .ToList(),
                Groups = data.Groups.Select(
                    g => new GroupOrUserPermissions(
                        g.Subject,
                        ParsePermissionType(g.Permission)))
                    .ToList()
            };
        }

        string GetGetFolderPermissionsQuery(List<string> users, List<string> groups)
        {
            var queryParams = new List<string>();

            if (users != null && users.Count > 0)
            {
                queryParams.Add("users=" + string.Join("|", users));
            }

            if (groups != null && groups.Count > 0)
            {
                queryParams.Add("groups=" + string.Join("|", groups));
            }

            return string.Join("&", queryParams);
        }

        string GetSetFolderPermissionsContent(PermissionType type, List<string> users, List<string> groups)
        {
            var builder = new StringBuilder();
            builder
                .Append("{");

            if (users != null && users.Count > 0)
            {
                builder.Append("\"users\": [")
                    .Append(string.Join(",", users.Select(u => "\"" + u + "\"")))
                    .Append("],");
            }

            if (groups != null && groups.Count > 0)
            {
                builder.Append("\"groups\": [")
                    .Append(string.Join(",", groups.Select(g => "\"" + g + "\"")))
                    .Append("],");
            }

            builder.Append("\"permission\": \"" + GetPermissionName(type) + "\"")
                .Append("}");
            return builder.ToString();
        }

        string GetPermissionName(PermissionType type)
        {
            switch(type)
            {
                case PermissionType.Viewer:
                    return "Viewer";
                case PermissionType.Editor:
                    return "Editor";
                case PermissionType.Full:
                    return "Full";
                case PermissionType.Owner:
                    return "Owner";

                default:
                    return "None";
            }
        }

        PermissionType ParsePermissionType(string permission)
        {
            switch (permission.ToLower())
            {
                case "owner":
                    return PermissionType.Owner;
                case "full":
                    return PermissionType.Full;
                case "editor":
                    return PermissionType.Editor;
                case "viewer":
                    return PermissionType.Viewer;
                default:
                    return PermissionType.None;
            }
        }
    }
}
