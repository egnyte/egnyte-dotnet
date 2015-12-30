namespace Egnyte.Api.Files
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Egnyte.Api.Common;

    public class FilesClient
    {
        private const string FilesBasePath = "https://{0}.egnyte.com/pubapi/v1/fs/";

        private readonly HttpClient httpClient;

        private readonly string domain;

        internal FilesClient(HttpClient httpClient, string domain)
        {
            this.httpClient = httpClient;
            this.domain = domain;
        }

        /// <summary>
        /// List information about a file or folder (including folder contents).
        /// </summary>
        /// <param name="path">Full path to folder or file</param>
        /// <param name="listContent">If false, then do not include contents of folder in response</param>
        /// <param name="allowedLinkTypes">If true, then show allowed_file_link_types,
        /// allowed_folder_link_types fields, and allow_upload_links fields.</param>
        /// <returns>Metadata info about file or folder</returns>
        public async Task<FileOrFolderMetadata> ListFileOrFolder(
            string path,
            bool listContent = true,
            bool allowedLinkTypes = false)
        {
            var listFilesUri = PrepareListFileOrFolderUri(path, listContent, allowedLinkTypes);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, listFilesUri);
            var serviceHandler = new ServiceHandler<ListFileOrFolderResponse>(httpClient);

            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return FilesHelper.MapResponseToMetadata(response);
        }

        private Uri PrepareListFileOrFolderUri(string path, bool listContent, bool allowedLinkTypes)
        {
            var query = "list_content=" + listContent + "&allowed_link_types=" + allowedLinkTypes;
            var uriBuilder = new UriBuilder(string.Format(FilesBasePath, domain) + path) { Query = query };

            return uriBuilder.Uri;
        }
    }
}
