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
    using Tasks;
    using Egnyte.Api.ProjectFolders;

    public class EgnyteClient
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="token">OAuth 2.0 token obtained from Egnyte</param>
        /// <param name="domain">Domain on which you connect to egnyte,
        /// i.e.: domain is 'mydomain', when url looks like: mydomain.egnyte.com</param>
        /// <param name="httpClient">You can provide your own httpClient. Optional</param>
        /// <param name="requestTimeout">You can provide timeout for calling Egnyte API,
        /// by default it's 10 minutes. This parameter is optional</param>
        /// <param name="host">Full host name on which you connect to egnyte,
        /// i.e.: host is 'my.custom.host.com', when url looks like: my.custom.host.com</param>
        public EgnyteClient(
            string token,
            string domain = "",
            HttpClient httpClient = null,
            TimeSpan? requestTimeout = null,
            string host = "")
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (string.IsNullOrWhiteSpace(domain) && string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentNullException("domain", "Domain or host has to specified");
            }

            httpClient = httpClient ?? new HttpClient();

            httpClient.Timeout = TimeSpan.FromMinutes(10);
            if (requestTimeout.HasValue)
            {
                httpClient.Timeout = requestTimeout.Value;
            }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            Files = new FilesClient(httpClient, domain, host);
            Users = new UsersClient(httpClient, domain, host);
            Links = new LinksClient(httpClient, domain, host);
            Groups = new GroupsClient(httpClient, domain, host);
            Permissions = new PermissionsClient(httpClient, domain, host);
            ProjectFolders = new ProjectFoldersClient(httpClient, domain, host);
            Search = new SearchClient(httpClient, domain, host);
            Audit = new AuditClient(httpClient, domain, host);
            Tasks = new TasksClient(httpClient, domain, host);
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
        /// The Project Folder API allows you to search, create, modify, and delete Project folders
        /// </summary>
        public ProjectFoldersClient ProjectFolders { get; private set; }

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

        /// <summary>
        /// The Tasks API allows you to create a task, list all tasks for a file or for a user,
        /// and list the details for a specific task. Tasks API can also be referred to as "Workflow API".
        /// </summary>
        public TasksClient Tasks { get; private set; }
    }
}