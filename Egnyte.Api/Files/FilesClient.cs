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
        /// <returns>Metadata info about file or folder</returns>
        public async Task<FileOrFolderMetadata> ListFileOrFolder(string path)
        {
            var listFilesUri = new Uri(string.Format(FilesBasePath, domain) + path);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, listFilesUri);
            var serviceHandler = new ServiceHandler<ListFileOrFolderResponse>(httpClient);

            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return FilesHelper.MapResponseToMetadata(response);
        }
    }
}
