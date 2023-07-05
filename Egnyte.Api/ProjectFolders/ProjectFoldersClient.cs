﻿using Egnyte.Api.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Egnyte.Api.ProjectFolders
{
    public class ProjectFoldersClient : BaseClient
    {
        const string ProjectFoldersMethod = "/pubapi/v1/project-folders";
        const string ProjectFoldersMethodV2 = "/pubapi/v2/project-folders";

        internal ProjectFoldersClient(HttpClient httpClient, string domain = "", string host = "") : base(httpClient, domain, host) { }

        /// <summary>
        /// List all project folders in the domain.
        /// </summary>
        /// <returns>List of all projects</returns>
        public async Task<List<ProjectDetails>> GetAllProjects()
        {
            var uriBuilder = BuildUri(ProjectFoldersMethodV2);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

            var serviceHandler = new ServiceHandler<List<ProjectDetails>>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return response.Data;
        }

        /// <summary>
        /// Marks an existing folder as a project.
        /// </summary>
        /// <param name="rootFolderId">FolderId of the root project folder</param>
        /// <param name="name">The name of the project</param>
        /// <param name="description">Optional. Folder description</param>
        /// <param name="status">Status of the project. Possible values: pending, in-progress, or on-hold</param>
        /// <param name="startDate">Optional. The start date of the project</param>
        /// <param name="completionDate">Optional. The completion date of the project</param>
        /// <returns>True if succeeded, False otherwise</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<bool> MarkFolderAsProject(
            string rootFolderId,
            string name,
            string status,
            string description = null,
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
                    MapMarkFolderAsProjectRequest(
                        rootFolderId,
                        name,
                        status,
                        description,
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

            var uriBuilder = BuildUri(ProjectFoldersMethodV2 + "/" + projectId);
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
        public async Task<List<ProjectDetails>> FindProjectByRootFolderId(string rootFolderId)
        {
            if (string.IsNullOrWhiteSpace(rootFolderId))
            {
                throw new ArgumentNullException(nameof(rootFolderId));
            }

            var uriBuilder = BuildUri(ProjectFoldersMethodV2 + "/search");
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
            {
                Content = new StringContent(MapRootFolderIdForRequest(rootFolderId), Encoding.UTF8, "application/json")
            };

            var serviceHandler = new ServiceHandler<List<ProjectDetails>>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return response.Data;
        }

        /// <summary>
        /// Create a project from template
        /// </summary>
        /// <param name="parentFolderId">Folder ID of the parent folder where the project folder will be created</param>
        /// <param name="templateFolderId">Folder ID of the project folder template</param>
        /// <param name="folderName">Name of the new folder that will be created</param>
        /// <param name="name">The name of the project</param>
        /// <param name="description">Optional. Folder description</param>
        /// <param name="status">Status of the project. Possible values: pending, in-progress, or on-hold</param>
        /// <param name="projectId">ID of the project</param>
        /// <param name="customerName">Optional. The customer associated with the project</param>
        /// <param name="location">Optional. Address</param>
        /// <param name="startDate">Optional. The start date of the project</param>
        /// <param name="completionDate">Optional. The completion date of the project</param>
        /// <returns>If your template creates new groups, those groups will be included in the response body.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<CreateFromTemplateResponse> CreateProjectFromTemplate(
            string parentFolderId,
            string templateFolderId,
            string folderName,
            string name,
            string status,
            string projectId,
            string customerName = null,
            ProjectFolderLocation location = null,
            string description = null,
            DateTime? startDate = null,
            DateTime? completionDate = null)
        {
            if (string.IsNullOrWhiteSpace(parentFolderId))
            {
                throw new ArgumentNullException(nameof(parentFolderId));
            }
            if (string.IsNullOrWhiteSpace(templateFolderId))
            {
                throw new ArgumentNullException(nameof(templateFolderId));
            }
            if (string.IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentNullException(nameof(folderName));
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentNullException(nameof(status));
            }
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentNullException(nameof(projectId));
            }

            var uriBuilder = BuildUri(ProjectFoldersMethodV2);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
            {
                Content = new StringContent(
                    MapCreateProjectRequest(
                        parentFolderId,
                        templateFolderId,
                        folderName,
                        name,
                        status,
                        description,
                        projectId,
                        customerName,
                        location,
                        startDate,
                        completionDate),
                    Encoding.UTF8,
                    "application/json")
            };

            try
            {
                var serviceHandler = new ServiceHandler<CreateFromTemplateResponse>(httpClient);
                var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);
                return response.Data;
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

            var uriBuilder = BuildUri(ProjectFoldersMethodV2 + "/" + projectId);
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
        /// <param name="name">The name of the project</param>
        /// <param name="projectId">ID of the project</param>
        /// <param name="status">Status of the project. Possible values: pending, in-progress, or on-hold</param>
        /// <param name="description">Optional. Folder description</param>
        /// <param name="customerName">Optional. The customer associated with the project</param>
        /// <param name="location">Optional. Address</param>
        /// <param name="startDate">Optional. The start date of the project</param>
        /// <param name="completionDate">Optional. The completion date of the project</param>
        /// <returns>True if succeeded, False otherwise</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<bool> UpdateProject(
            string name,
            string projectId,
            string status,
            string description = null,
            string customerName = null,
            ProjectFolderLocation location = null,
            DateTime? startDate = null,
            DateTime? completionDate = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentNullException(nameof(projectId));
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentNullException(nameof(status));
            }

            var uriBuilder = BuildUri(ProjectFoldersMethodV2 + "/" + projectId);
            var httpRequest = new HttpRequestMessage(new HttpMethod("PATCH"), uriBuilder.Uri)
            {
                Content = new StringContent(
                    MapCreateProjectRequest(
                        null,
                        null,
                        null,
                        name,
                        status,
                        description,
                        projectId,
                        customerName,
                        location,
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
        /// <param name="projectId">Project Id</param>
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

        private string MapCreateProjectRequest(
            string parentFolderId,
            string templateFolderId,
            string folderName,
            string name,
            string status,
            string description,
            string projectId,
            string customerName,
            ProjectFolderLocation location,
            DateTime? startDate,
            DateTime? completionDate)
        {
            var builder = new StringBuilder();
            builder.Append("{");
            if (!string.IsNullOrWhiteSpace(parentFolderId))
            {
                builder.Append("\"parentFolderId\" : \"" + parentFolderId + "\",");
            }
            if (!string.IsNullOrWhiteSpace(templateFolderId))
            {
                builder.Append("\"templateFolderId\" : \"" + templateFolderId + "\",");
            }
            if (!string.IsNullOrWhiteSpace(folderName))
            {
                builder.Append("\"folderName\" : \"" + folderName + "\",");
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                builder.Append("\"name\" : \"" + name + "\",");
            }
            if (!string.IsNullOrWhiteSpace(status))
            {
                builder.Append("\"status\" : \"" + status + "\",");
            }
            if (!string.IsNullOrWhiteSpace(description))
            {
                builder.Append("\"description\" : \"" + description + "\",");
            }
            if (!string.IsNullOrWhiteSpace(projectId))
            {
                builder.Append("\"projectId\" : \"" + projectId + "\",");
            }
            if (!string.IsNullOrWhiteSpace(customerName))
            {
                builder.Append("\"customerName\" : \"" + customerName + "\",");
            }
            if (location != null)
            {
                if (!string.IsNullOrWhiteSpace(location.StreetAddress1))
                {
                    builder.Append("\"location.streetAddress1\" : \"" + location.StreetAddress1 + "\",");
                }
                if (!string.IsNullOrWhiteSpace(location.StreetAddress2))
                {
                    builder.Append("\"location.streetAddress2\" : \"" + location.StreetAddress2 + "\",");
                }
                if (!string.IsNullOrWhiteSpace(location.City))
                {
                    builder.Append("\"location.city\" : \"" + location.City + "\",");
                }
                if (!string.IsNullOrWhiteSpace(location.State))
                {
                    builder.Append("\"location.state\" : \"" + location.State + "\",");
                }
                if (!string.IsNullOrWhiteSpace(location.PostalCode))
                {
                    builder.Append("\"location.postalCode\" : \"" + location.PostalCode + "\",");
                }
                if (!string.IsNullOrWhiteSpace(location.Country))
                {
                    builder.Append("\"location.country\" : \"" + location.Country + "\",");
                }
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

        private string MapMarkFolderAsProjectRequest(
            string rootFolderId,
            string name,
            string status,
            string description,
            DateTime? startDate,
            DateTime? completionDate)
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
            if (!string.IsNullOrWhiteSpace(description))
            {
                builder.Append("\"description\" : \"" + description + "\",");
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
                .Append("\"parentFolderId\" : \"" + rootFolderId + "\"")
                .Append("}");
            return builder.ToString();
        }

        private string MapCleanProjectRequest(bool deleteLinks, List<long> usersToDelete, List<long> usersToDisable)
        {
            var builder = new StringBuilder();
            builder
                .Append("{")
                .Append("\"deleteLinks\" : " + deleteLinks + ",");
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
                builder.Append("\"usersToDisable\" : [");
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
