using Egnyte.Api.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Egnyte.Api.Permissions
{
    public class PermissionsClient : BaseClient
    {
        const string PermissionsMethodV1 = "/pubapi/v1/perms";
        const string PermissionsMethod = "/pubapi/v2/perms";

        internal PermissionsClient(HttpClient httpClient, string domain = "", string host = "") : base(httpClient, domain, host) { }

        /// <summary>
        /// Sets the effective permission level for specific users or groups on a given folder
        /// </summary>
        /// <param name="path">Required. Full path to the folder</param>
        /// <param name="userPerms">List of usernames to set permissions for
        /// At least one user or group must be specified</param>
        /// <param name="groupPerms">List of groupnames to set permissions for
        /// At least one user or group must be specified</param>
        /// <param name="inheritsPermissions">Whether permissions should be inherited from the parent folder</param>
        /// <param name="keepParentPermissions">When disabling permissions inheritance with inheritsPermissions,
        /// this options determines whether previously inherited permissions from parent folders should be copied to this folder.</param>
        /// <returns>True if operation succedes</returns>
        public async Task<bool> SetFolderPermissions(
            string path,
            List<GroupOrUserPermissions> userPerms = null,
            List<GroupOrUserPermissions> groupPerms = null,
            bool? inheritsPermissions = null,
            bool? keepParentPermissions = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if ((userPerms == null || userPerms.Count == 0) && (groupPerms == null || groupPerms.Count == 0))
            {
                throw new ArgumentException("One of parameters: " + nameof(userPerms) + " or " + nameof(groupPerms) + " must be not empty.");
            }

            var uriBuilder = BuildUri(PermissionsMethod + "/" + path);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
            {
                Content = new StringContent(
                    GetSetFolderPermissionsContent(userPerms, groupPerms, inheritsPermissions, keepParentPermissions),
                    Encoding.UTF8,
                    "application/json")
            };

            var serviceHandler = new ServiceHandler<string>(httpClient);
            await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Gets the permissions for a given folder
        /// </summary>
        /// <param name="path">Required. Full path to the folder</param>
        /// <param name="users">Optional. List of usernames to report on
        /// If neither users nor groups is set then permissions for all subjects are returned</param>
        /// <param name="groups">Optional. List of groups to report on
        /// If neither users nor groups is set then permissions for all subjects are returned</param>
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

            var uriBuilder = BuildUri(PermissionsMethod + "/" + path);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

            var serviceHandler = new ServiceHandler<FolderPermissionsResponse>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return MapFolderPermissions(response.Data, users, groups);
        }

        /// <summary>
        /// Gets the effective permissions for a user for a given folder.
        /// This effective permission takes into account both user and group permissions
        /// that apply to the given user, along with permission inheritance
        /// </summary>
        /// <param name="username">Required. Egnyte username</param>
        /// <param name="path">Required. Full path to the folder</param>
        /// <returns>Effective user permissions for folder</returns>
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

            var uriBuilder = BuildUri(PermissionsMethodV1 + "/user/" + username, "folder=" + path);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

            var serviceHandler = new ServiceHandler<EffectivePermissionsResponse>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return ParsePermissionType(response.Data.Permission);
        }

        FolderPermissions MapFolderPermissions(FolderPermissionsResponse data, List<string> users, List<string> groups)
        {
            return new FolderPermissions(
                data.Users.Where(w => users == null || (users != null && users.Contains(w.Subject)))
                .Select(
                    u => new GroupOrUserPermissions(
                        u.Subject,
                        ParsePermissionType(u.Permission)))
                    .ToList(),
                data.Groups.Where(w => groups == null || (groups != null && groups.Contains(w.Subject)))
                .Select(
                    g => new GroupOrUserPermissions(
                        g.Subject,
                        ParsePermissionType(g.Permission)))
                    .ToList());
        }

        string GetSetFolderPermissionsContent(List<GroupOrUserPermissions> users, List<GroupOrUserPermissions> groups, bool? inheritsPermissions, bool? keepParentPermissions)
        {
            var builder = new StringBuilder();
            builder
                .Append("{");

            if (users != null && users.Count > 0)
            {
                builder.Append("\"userPerms\": {")
                    .Append(string.Join(",", users.Select(p => "\"" + p.Subject + "\": \"" + p.Permission + "\"")))
                    .Append("}");
            }

            if (groups != null && groups.Count > 0)
            {
                if (users != null && users.Count > 0)
                    builder.Append(",");
                builder.Append("\"groupPerms\": {")
                    .Append(string.Join(",", groups.Select(p => "\"" + p.Subject + "\": \"" + p.Permission + "\"")))
                    .Append("}");
            }

            if (inheritsPermissions.HasValue)
                builder.Append(",\"inheritsPermissions\": \"" + (inheritsPermissions.Value ? "true" : "false") + "\"");
            
            if (keepParentPermissions.HasValue)
                builder.Append(",\"keepParentPermissions\": \"" + (keepParentPermissions.Value ? "true" : "false") + "\"");

            builder.Append("}");
            return builder.ToString();
        }

        string GetPermissionName(PermissionType type)
        {
            switch (type)
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
