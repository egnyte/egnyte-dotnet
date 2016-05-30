namespace Egnyte.Api
{
    using System.Net.Http;
    using System.Net.Http.Headers;

    using Egnyte.Api.Files;
    using System;
    using Users;
    using Links;
    using Groups;
    using Permissions;
    using Search;
    using Audit;

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
            Permissions = new PermissionsClient(httpClient, domain);
            Search = new SearchClient(httpClient, domain);
            Audit = new AuditClient(httpClient, domain);
        }

        /// <summary>
        /// Files allows you to perform the normal file system actions: create, update, move, copy, delete,
        /// download, and list information about files and folders
        /// </summary>
        public FilesClient Files { get; private set; }

        /// <summary>
        /// Users allows you to create, update, get information about, and delete users.
        /// You can customize settings like user role or authentication type and even control whether a new user
        /// receives an invitation email
        /// </summary>
        public UsersClient Users { get; private set; }

        /// <summary>
        /// Links allows you to list the file and folder links in your domain,
        /// show the details of an individual link, create new links, and delete links
        /// </summary>
        public LinksClient Links { get; private set; }

        /// <summary>
        /// Groups allows you to list the groups in your domain, show the details of an individual group,
        /// manage group membership, create new groups, rename groups, and delete groups.
        /// This API is designed to comply with SCIM 1.1
        /// </summary>
        public GroupsClient Groups { get; private set; }

        /// <summary>
        /// With Permissions you can list, set, and remove folder permissions for users and groups.
        /// Refer to the table here: https://developers.egnyte.com/docs/read/Egnyte_Permissions_API#Permissions
        /// for details on our different permission levels
        /// </summary>
        public PermissionsClient Permissions { get; private set; }

        /// <summary>
        /// The Search API allows you to find content stored in Egnyte based on filenames,
        /// metadata, and text content. Searches are performed in the context of the user token being passed.
        /// Accordingly, a user will only see results for contents they have permission to access.
        /// Note, this endpoint is currently limited to searching for files only
        /// </summary>
        public SearchClient Search { get; private set; }

        /// <summary>
        /// The Audit Reporting API allows you to programmatically generate and retrieve reports
        /// on login activity, file actions, and permission changes. The scope of these reports
        /// effectively gives you a 360° view of the activity in your account
        /// </summary>
        public AuditClient Audit { get; private set; }
    }
}
