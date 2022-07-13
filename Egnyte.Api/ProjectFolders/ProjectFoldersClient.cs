using Egnyte.Api.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Egnyte.Api.ProjectFolders
{
    public class ProjectFoldersClient : BaseClient
    {
        const string ProjectFoldersMethod = "/pubapi/v1/project-folders";

        internal ProjectFoldersClient(HttpClient httpClient, string domain = "", string host = "") : base(httpClient, domain, host) { }

        /// <summary>
        /// List all project folders in the domain.
        /// </summary>
        /// <returns>List of all projects</returns>
        public async Task<ProjectsList> GetAllProjects()
        {
            var uriBuilder = BuildUri(ProjectFoldersMethod);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

            var serviceHandler = new ServiceHandler<List<ProjectDetails>>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return new ProjectsList(response.Data, response.Data.Count);
        }

        /// <summary>
        /// Retrieve a project based on its Id
        /// </summary>
        /// <param name="projectId">Project Id</param>
        /// <returns>Project details</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<ProjectDetails> FindProjectById(string projectId)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentNullException(nameof(projectId));
            }

            var uriBuilder = BuildUri(ProjectFoldersMethod + "/" + projectId);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

            var serviceHandler = new ServiceHandler<ProjectDetails>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return response.Data;
        }

        /// <summary>
        /// Retrieve a project based on the id of the root folder
        /// </summary>
        /// <param name="rootFolderId">Root folder Id</param>
        /// <returns>List of all projects under provided folder</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<ProjectsList> FindProjectByRootFolderId(string rootFolderId)
        {
            if (string.IsNullOrWhiteSpace(rootFolderId))
            {
                throw new ArgumentNullException(nameof(rootFolderId));
            }

            var uriBuilder = BuildUri(ProjectFoldersMethod + "/search");
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
            {
                Content = new StringContent(MapRootFolderIdForRequest(rootFolderId), Encoding.UTF8, "application/json")
            };

            var serviceHandler = new ServiceHandler<List<ProjectDetails>>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return new ProjectsList(response.Data, response.Data.Count);
        }

        /// <summary>
        /// Create a project
        /// </summary>
        /// <param name="rootFolderId">FolderId of the root project folder</param>
        /// <param name="name">The name of the project</param>
        /// <param name="desc">Optional. Folder description</param>
        /// <param name="status">Status of the project. Possible values: pending, in-progress, completed, on-hold, or canceled</param>
        /// <param name="startDate">Optional. The start date of the project</param>
        /// <param name="completionDate">Optional. The completion date of the project</param>
        /// <returns>Created project Id</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<string> CreateProject(
            string rootFolderId,
            string name,
            string desc = null,
            string status = "pending",
            DateTime? startDate = null,
            DateTime? completionDate = null)
        {
            if (string.IsNullOrWhiteSpace(rootFolderId))
            {
                throw new ArgumentNullException(nameof(rootFolderId));
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentNullException(nameof(status));
            }

            var uriBuilder = BuildUri(ProjectFoldersMethod);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
            {
                Content = new StringContent(
                    MapCreateProjectRequest(
                        rootFolderId,
                        name,
                        desc,
                        status,
                        startDate,
                        completionDate),
                    Encoding.UTF8,
                    "application/json")
            };

            try
            {
                var serviceHandler = new ServiceHandler<ProjectCreatedResponse>(httpClient);
                var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);
                return response.Data.ProjectId;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Remove a project
        /// </summary>
        /// <param name="projectId">Project Id</param>
        /// <returns>True if succeeded, False otherwise</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<bool> RemoveProject(string projectId)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentNullException(nameof(projectId));
            }

            var uriBuilder = BuildUri(ProjectFoldersMethod + "/" + projectId);
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, uriBuilder.Uri);

            try
            {
                var serviceHandler = new ServiceHandler<object>(httpClient);
                var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Update existing project
        /// </summary>
        /// <param name="projectId">Project Id</param>
        /// <param name="name">The name of the project</param>
        /// <param name="desc">Optional. Folder description</param>
        /// <param name="status">Status of the project. Possible values: pending, in-progress, completed, on-hold, or canceled</param>
        /// <param name="startDate">Optional. The start date of the project</param>
        /// <param name="completionDate">Optional. The completion date of the project</param>
        /// <returns>True if succeeded, False otherwise</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<bool> UpdateProject(
            string projectId,
            string name,
            string desc = null,
            string status = "pending",
            DateTime? startDate = null,
            DateTime? completionDate = null)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentNullException(nameof(projectId));
            }

            var uriBuilder = BuildUri(ProjectFoldersMethod + "/" + projectId);
            var httpRequest = new HttpRequestMessage(new HttpMethod("PATCH"), uriBuilder.Uri)
            {
                Content = new StringContent(
                    MapCreateProjectRequest(
                        null,
                        name,
                        desc,
                        status,
                        startDate,
                        completionDate),
                    Encoding.UTF8,
                    "application/json")
            };

            try
            {
                var serviceHandler = new ServiceHandler<object>(httpClient);
                var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Clean up a project
        /// </summary>
        /// <param name="projectId">Projecr Id</param>
        /// <param name="deleteLinks">If set to true, all existing active links in the project will be deleted</param>
        /// <param name="usersToDelete">List of User Ids to be deleted</param>
        /// <param name="usersToDisable">List of User Ids to be disabled</param>
        /// <returns>True if succeeded</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<bool> CleanUpProject(
            string projectId,
            bool deleteLinks,
            List<long> usersToDelete = null,
            List<long> usersToDisable = null)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentNullException(nameof(projectId));
            }

            var uriBuilder = BuildUri(ProjectFoldersMethod + "/" + projectId + "/cleanup");
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
            {
                Content = new StringContent(
                    MapCleanProjectRequest(deleteLinks, usersToDelete, usersToDisable),
                    Encoding.UTF8,
                    "application/json")
            };

            var serviceHandler = new ServiceHandler<object>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);
            return true;
        }

        private string MapCreateProjectRequest(string rootFolderId, string name, string desc, string status, DateTime? startDate, DateTime? completionDate)
        {
            var builder = new StringBuilder();
            builder.Append("{");
            if (!string.IsNullOrWhiteSpace(rootFolderId))
            {
                builder.Append("\"rootFolderId\" : \"" + rootFolderId + "\",");
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                builder.Append("\"name\" : \"" + name + "\",");
            }
            if (!string.IsNullOrWhiteSpace(status))
            {
                builder.Append("\"status\" : \"" + status + "\",");
            }
            if (!string.IsNullOrWhiteSpace(desc))
            {
                builder.Append("\"description\" : \"" + desc + "\",");
            }
            if (startDate.HasValue)
            {
                builder.Append("\"startDate\" : \"" + startDate.Value.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss.000+0000") + "\",");
            }
            if (completionDate.HasValue)
            {
                builder.Append("\"completionDate\" : \"" + completionDate.Value.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss.000+0000") + "\",");
            }
            builder.Length -= 1;
            builder.Append("}");
            return builder.ToString().TrimEnd(',');
        }

        string MapRootFolderIdForRequest(string rootFolderId)
        {
            var builder = new StringBuilder();
            builder
                .Append("{")
                .Append("\"rootFolderId\" : \"" + rootFolderId + "\"")
                .Append("}");
            return builder.ToString();
        }

        private string MapCleanProjectRequest(bool deleteLinks, List<long> usersToDelete, List<long> usersToDisable)
        {
            var builder = new StringBuilder();
            builder.Append("{").Append("\"deleteLinks\" : " + deleteLinks + ",");
            if (usersToDelete != null && usersToDelete.Count > 0)
            {
                builder.Append("\"usersToDelete\" : [");
                foreach (var userToDelete in usersToDelete)
                {
                    builder.Append(userToDelete).Append(",");
                }
                builder.Length -= 1;
                builder.Append("],");
            }
            if (usersToDisable != null && usersToDisable.Count > 0)
            {
                builder.Append("\"usersToDelete\" : [");
                foreach (var userToDisable in usersToDisable)
                {
                    builder.Append(userToDisable).Append(",");
                }
                builder.Length -= 1;
                builder.Append("],");
            }
            builder.Length -= 1;
            builder.Append("}");
            return builder.ToString().TrimEnd(',');
        }
    }
}
