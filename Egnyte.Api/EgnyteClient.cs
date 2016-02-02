namespace Egnyte.Api
{
    using System.Net.Http;
    using System.Net.Http.Headers;

    using Egnyte.Api.Files;
    using System;
    using Users;
    using Links;
    using Groups;
    public class EgnyteClient
    {
        public EgnyteClient(
            string token,
            string domain,
            HttpClient httpClient = null,
            TimeSpan? requestTimeout = null)
        {
            httpClient = httpClient ?? new HttpClient();

            httpClient.Timeout = TimeSpan.FromMinutes(10);
            if (requestTimeout.HasValue)
            {
                httpClient.Timeout = requestTimeout.Value;
            }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            Files = new FilesClient(httpClient, domain);
            Users = new UsersClient(httpClient, domain);
            Links = new LinksClient(httpClient, domain);
            Groups = new GroupsClient(httpClient, domain);
        }

        /// <summary>
        /// Files allows you to perform the normal file system actions: create, update, move, copy, delete,
        /// download, and list information about files and folders.
        /// </summary>
        public FilesClient Files { get; private set; }

        /// <summary>
        /// Users allows you to create, update, get information about, and delete users.
        /// You can customize settings like user role or authentication type and even control whether a new user
        /// receives an invitation email.
        /// </summary>
        public UsersClient Users { get; private set; }

        /// <summary>
        /// Links allows you to list the file and folder links in your domain,
        /// show the details of an individual link, create new links, and delete links.
        /// </summary>
        public LinksClient Links { get; private set; }

        /// <summary>
        /// Groups allows you to list the groups in your domain, show the details of an individual group,
        /// manage group membership, create new groups, rename groups, and delete groups.
        /// This API is designed to comply with SCIM 1.1.
        /// </summary>
        public GroupsClient Groups { get; private set; }
    }
}
