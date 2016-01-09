namespace Egnyte.Api.Files
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Egnyte.Api.Common;

    public class FilesClient
    {
        private const string FilesBasePath = "https://{0}.egnyte.com/pubapi/v1/fs/";

        private const string FilesContentBasePath = "https://{0}.egnyte.com/pubapi/v1/fs-content/";

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

            return FilesHelper.MapResponseToMetadata(response.Data);
        }

        /// <summary>
        /// Creates a folder for specified path
        /// </summary>
        /// <param name="path">Full path to the new folder</param>
        /// <returns>Returns true if creating of a folder succeeded</returns>
        public async Task<bool> CreateFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException("path");
            }

            var uriBuilder = new UriBuilder(string.Format(FilesBasePath, domain) + path);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
            {
                Content = new StringContent(@"{""action"": ""add_folder""}", Encoding.UTF8, "application/json")
            };
            
            var serviceHandler = new ServiceHandler<string>(httpClient);
            await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Creates or updates a file.
        /// </summary>
        /// <param name="path">Full path to the file</param>
        /// <param name="file">Content of a file in a memory stream</param>
        /// <returns>Response with checksum and ids.
        /// Checksum is a SHA512 hash of entire file that can be used for validating upload integrity.</returns>
        public async Task<CreateOrUpdateFile> CreateOrUpdateFile(string path, MemoryStream file)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException("path");
            }

            if (file.Length == 0)
            {
                throw new ArgumentNullException("file");
            }

            var uriBuilder = new UriBuilder(string.Format(FilesContentBasePath, domain) + path);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
            {
                Content = new StreamContent(file)
            };

            var serviceHandler = new ServiceHandler<CreateOrUpdateFileResponse>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return new CreateOrUpdateFile(
                response.Headers.ContainsKey("X-Sha512-Checksum")
                    ? response.Headers["X-Sha512-Checksum"]
                    : response.Data.Checksum,
                response.Headers.ContainsKey("Last-Modified")
                    ? DateTime.Parse(response.Headers["Last-Modified"])
                    : DateTime.Now,
                response.Headers.ContainsKey("ETag")
                    ? response.Headers["ETag"]
                    : response.Data.EntryId);
        }

        /// <summary>
        /// Copies a file or folder
        /// </summary>
        /// <param name="path">Path to file or folder to copy</param>
        /// <param name="destination">Full path where file/folder will be copied</param>
        /// <returns>Returns true if moving file or folder succeeded</returns>
        public async Task<bool> MoveFileOrFolder(string path, string destination)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException("path");
            }

            if (string.IsNullOrWhiteSpace(destination))
            {
                throw new ArgumentNullException("destination");
            }

            if (!destination.StartsWith("/"))
            {
                destination = "/" + destination;
            }

            var uriBuilder = new UriBuilder(string.Format(FilesBasePath, domain) + path);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
            {
                Content = new StringContent(
                    string.Format(@"{{""action"": ""move"", ""destination"": ""{0}""}}", destination),
                    Encoding.UTF8,
                    "application/json")
            };

            var serviceHandler = new ServiceHandler<string>(httpClient);
            await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return true;
        }

        public async Task<DownloadedFile> DownloadFile(string path)
        {
            var listFilesUri = PrepareDownloadFileUri(path);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, listFilesUri);
            var serviceHandler = new ServiceHandler<string>(httpClient);

            var response = await serviceHandler.GetFileToDownload(httpRequest).ConfigureAwait(false);

            return MapResponseToDownloadedFile(response);
        }

        private DownloadedFile MapResponseToDownloadedFile(ServiceResponse<byte[]> response)
        {
            return new DownloadedFile
                       {
                           Data = response.Data,
                           Checksum = 
                               response.Headers.ContainsKey("X-Sha512-Checksum")
                                   ? response.Headers["X-Sha512-Checksum"]
                                   : string.Empty,
                           LastModified =
                               response.Headers.ContainsKey("Last-Modified")
                                   ? DateTime.Parse(response.Headers["Last-Modified"])
                                   : DateTime.Now,
                           ETag = 
                               response.Headers.ContainsKey("ETag")
                                   ? response.Headers["ETag"]
                                   : string.Empty,
                           ContentType = 
                               response.Headers.ContainsKey("Content-Type")
                                   ? response.Headers["Content-Type"]
                                   : string.Empty,
                           ContentLength =
                               response.Headers.ContainsKey("Content-Length")
                                   ? int.Parse(response.Headers["Content-Length"])
                                   : 0
                       };
        }

        private Uri PrepareListFileOrFolderUri(string path, bool listContent, bool allowedLinkTypes)
        {
            var query = "list_content=" + listContent + "&allowed_link_types=" + allowedLinkTypes;
            var uriBuilder = new UriBuilder(string.Format(FilesBasePath, domain) + path) { Query = query };

            return uriBuilder.Uri;
        }

        private Uri PrepareDownloadFileUri(string path)
        {
            var query = string.Empty;
            //var query = "list_content=" + listContent + "&allowed_link_types=" + allowedLinkTypes;
            var uriBuilder = new UriBuilder(string.Format(FilesContentBasePath, domain) + path) { Query = query };

            return uriBuilder.Uri;
        }
    }
}
