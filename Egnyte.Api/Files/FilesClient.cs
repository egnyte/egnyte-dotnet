using System.Linq;

namespace Egnyte.Api.Files
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Egnyte.Api.Common;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class FilesClient : BaseClient
    {
        const string FilesMethod = "/pubapi/v1/fs";

        const string FilesContentMethod = "/pubapi/v1/fs-content";

        const string FilesChunkedContentMethod = "/pubapi/v1/fs-content-chunked";
        
        internal FilesClient(HttpClient httpClient, string domain = "", string host = "") : base(httpClient, domain, host) { }

        /// <summary>
        /// Creates a folder for specified path
        /// </summary>
        /// <param name="path">Full path to the new folder</param>
        /// <returns>Returns true if creating of a folder succeeded</returns>
        public async Task<FolderCreatedResponse> CreateFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            var uriBuilder = BuildUri(FilesMethod + "/" + EncodePathSegments(path));
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
            {
                Content = new StringContent(@"{""action"": ""add_folder""}", Encoding.UTF8, "application/json")
            };
            
            var serviceHandler = new ServiceHandler<FolderCreatedResponse>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return response.Data;
        }

        /// <summary>
        /// Creates or updates a file. To upload files larger than 100 MB, use the ChunkedUpload method
        /// </summary>
        /// <param name="path">Full path to the file</param>
        /// <param name="file">Content of a file in a memory stream</param>
        /// <returns>Response with checksum and ids.
        /// Checksum is a SHA512 hash of entire file that can be used for validating upload integrity</returns>
        public async Task<UploadedFileMetadata> CreateOrUpdateFile(string path, MemoryStream file)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (file.Length == 0)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var uriBuilder = BuildUri(FilesContentMethod + "/" + EncodePathSegments(path));
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
            {
                Content = new StreamContent(file)
            };

            var serviceHandler = new ServiceHandler<CreateOrUpdateFileResponse>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return new UploadedFileMetadata(
                response.Headers.ContainsKey("x-sha512-checksum")
                    ? response.Headers["x-sha512-checksum"]
                    : response.Data.Checksum,
                response.Headers.ContainsKey("last-modified")
                    ? DateTime.Parse(response.Headers["last-modified"])
                    : DateTime.Now,
                response.Headers.ContainsKey("etag")
                    ? response.Headers["etag"]
                    : response.Data.EntryId);
        }

        /// <summary>
        /// Moves a file or folder
        /// </summary>
        /// <param name="path">Path to file or folder to move</param>
        /// <param name="destination">Full path where file/folder will be moved</param>
        /// <returns>Returns true if moving file or folder succeeded</returns>
        public async Task<bool> MoveFileOrFolder(string path, string destination)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (string.IsNullOrWhiteSpace(destination))
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (!destination.StartsWith("/", StringComparison.Ordinal))
            {
                destination = "/" + destination;
            }

            var uriBuilder = BuildUri(FilesMethod + "/" + EncodePathSegments(path));
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

        /// <summary>
        /// Copies a file or folder
        /// </summary>
        /// <param name="path">Path to file or folder to copy</param>
        /// <param name="destination">Full path where file/folder will be copied</param>
        /// <returns>Returns true if copying file or folder succeeded</returns>
        public async Task<bool> CopyFileOrFolder(string path, string destination)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (string.IsNullOrWhiteSpace(destination))
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (!destination.StartsWith("/", StringComparison.Ordinal))
            {
                destination = "/" + destination;
            }

            var uriBuilder = BuildUri(FilesMethod + "/" + EncodePathSegments(path));
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
            {
                Content = new StringContent(
                    string.Format(@"{{""action"": ""copy"", ""destination"": ""{0}""}}", destination),
                    Encoding.UTF8,
                    "application/json")
            };

            var serviceHandler = new ServiceHandler<string>(httpClient);
            await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Downloads a file (note that this method supports ranged downloads)
        /// </summary>
        /// <param name="path">Full path to the file</param>
        /// <param name="rangeOfBytes">The range of bytes to download, use of this header is optional</param>
        /// <param name="entryId">Specifies the entry ID of the file version to download.
        /// Entry IDs are shown in the detail listing for a file</param>
        /// <returns>File download information with data in bytes</returns>
        public async Task<DownloadedFile> DownloadFile(
            string path,
            Range rangeOfBytes = null,
            string entryId = null)
        {
            var listFilesUri = PrepareDownloadFileUri(path, entryId);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, listFilesUri);

            if (rangeOfBytes != null)
            {
                httpRequest.Headers.Add("Range", string.Format("bytes={0}-{1}", rangeOfBytes.From, rangeOfBytes.To));
            }
            
            var serviceHandler = new ServiceHandler<string>(httpClient);
            var response = await serviceHandler.GetFileToDownload(httpRequest).ConfigureAwait(false);

            return MapResponseToDownloadedFile(response);
        }

        /// <summary>
        /// Downloads a file as a stream (suitable for files larger then 2 GB)
        /// </summary>
        /// <param name="path">Full path to the file</param>
        /// <returns>File download information with data in a stream</returns>
        public async Task<DownloadedFileAsStream> DownloadFileAsStream(string path)
        {
            var listFilesUri = PrepareDownloadFileUri(path);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, listFilesUri);

            var serviceHandler = new ServiceHandler<string>(httpClient);
            var response = await serviceHandler.GetFileToDownloadAsStream(httpRequest).ConfigureAwait(false);

            return MapResponseToDownloadedFileAsStream(response);
        }

        /// <summary>
        /// List information about a file or folder (including folder contents)
        /// </summary>
        /// <param name="path">Full path to folder or file</param>
        /// <param name="listContent">If false, then do not include contents of folder in response</param>
        /// <param name="allowedLinkTypes">If true, then show allowed_file_link_types,
        /// allowed_folder_link_types fields, and allow_upload_links fields</param>
        /// <param name="listCustomMetadata">If true, the custom_metadata fields will be included</param>
        /// <param name="count">The maximum number of items to return</param>
        /// <param name="offset">The zero-based index from which to start returning items. This is used for paginating the folder listing</param>
        /// <param name="sortBy">The field that should be used for sorting</param>
        /// <param name="sortDirection">The direction of the sort</param>
        /// <returns>Metadata info about file or folder</returns>
        public async Task<FileOrFolderMetadata> ListFileOrFolder(
            string path,
            bool listContent = true,
            bool allowedLinkTypes = false,
            bool? listCustomMetadata = null,
            int? count = null,
            int? offset = null,
            string sortBy = null,
            string sortDirection = null)
        {
            var listFilesUri = PrepareListFileOrFolderUri(path, listContent, allowedLinkTypes, listCustomMetadata, count, offset, sortBy, sortDirection);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, listFilesUri);
            var serviceHandler = new ServiceHandler<ListFileOrFolderResponse>(httpClient);

            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return FilesHelper.MapResponseToMetadata(response.Data);
        }

        /// <summary>
        /// Deletes a file or folder.
        /// </summary>
        /// <param name="path">Full path to folder or file</param>
        /// <param name="entryId">Optional, entry id to a file</param>
        /// <returns>Returns true if deleting file or folder succeeded</returns>
        public async Task<bool> DeleteFileOrFolder(string path, string entryId = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            var query = string.Empty;
            if (!string.IsNullOrWhiteSpace(entryId))
            {
                query = "entry_id=" + entryId;
            }

            var uriBuilder = BuildUri(FilesMethod + "/" + EncodePathSegments(path), query);
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, uriBuilder.Uri);

            var serviceHandler = new ServiceHandler<string>(httpClient);
            await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// The chunked upload flow provides a mechanism to upload large files—we recommend only using this flow
        /// for files larger than 100 MB. To upload files of smaller sizes, you can use the CreateOrUpdateFile method
        /// </summary>
        /// <param name="path">Full path to file</param>
        /// <param name="file">Stream with the file content</param>
        /// <returns>Uploaded chunk metadata</returns>
        public async Task<ChunkUploadedMetadata> ChunkedUploadFirstChunk(string path, MemoryStream file)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (file.Length == 0)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var uriBuilder = BuildUri(FilesChunkedContentMethod + "/" + EncodePathSegments(path));
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
            {
                Content = new StreamContent(file)
            };
            httpRequest.Headers.Add("X-Egnyte-Chunk-Num", "1");

            // Experimental code to gather more information about failures
            httpRequest.RequestUri = ApplyAdditionalUrlMapping(httpRequest.RequestUri);
            var httpResponse = await httpClient.SendAsync(httpRequest).ConfigureAwait(false);
            var rawContent = httpResponse.Content != null ? await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false) : null;

            ExceptionHelper.CheckErrorStatusCode(httpResponse, rawContent);

            ServiceResponse<CreateOrUpdateFileResponse> response;
            try
            {
                response = new ServiceResponse<CreateOrUpdateFileResponse>
                {
                    Data = JsonConvert.DeserializeObject<CreateOrUpdateFileResponse>(rawContent),
                    Headers = httpResponse.GetLowercaseResponseHeaders()
                };
            }
            catch (Exception e)
            {
                throw new EgnyteApiException(
                    rawContent,
                    httpResponse,
                    e);
            }

            if (!response.Headers.ContainsKey("x-egnyte-chunk-num"))
            {
                var headerValues = string.Join(",", response.Headers.Select(h => $"{h.Key}:{h.Value}"));
                var message = $"Missing first chunk number header. Headers: {headerValues}, Content: {rawContent}";

                throw new EgnyteApiException(message);
            }

            return new ChunkUploadedMetadata(
                response.Headers.ContainsKey("x-egnyte-upload-id")
                    ? response.Headers["x-egnyte-upload-id"]
                    : string.Empty,
                response.Headers.ContainsKey("x-egnyte-chunk-num")
                    ? int.Parse(response.Headers["x-egnyte-chunk-num"])
                    : -1,
                response.Headers.ContainsKey("x-egnyte-chunk-sha512-checksum")
                    ? response.Headers["x-egnyte-chunk-sha512-checksum"]
                    : string.Empty);
        }

        private Uri ApplyAdditionalUrlMapping(Uri requestUri)
        {
            var url = requestUri.ToString();
            url = url.Replace("[", "%5B")
                .Replace("]", "%5D");
            return new Uri(url);
        }

        /// <summary>
        /// Method for uploading second and consecutive chunks of file. To upload first chunk use ChunkedUploadFirstChunk
        /// </summary>
        /// <param name="path">Full path to file</param>
        /// <param name="chunkNumber">Chunk number. First should be 1, others 2, 3, etc.</param>
        /// <param name="chunkUploadId">Chunk upload id from response after uploading first chunk</param>
        /// <param name="file">Stream with part of file content</param>
        /// <returns>Uploaded chunk metadata</returns>
        public async Task<ChunkUploadedMetadata> ChunkedUploadNextChunk(
            string path,
            int chunkNumber,
            string chunkUploadId,
            MemoryStream file)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (chunkNumber <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(chunkNumber), "Chunk number must start with 1 and next chunks should be 2, 3, etc.");
            }

            if (string.IsNullOrWhiteSpace(chunkUploadId))
            {
                throw new ArgumentNullException(nameof(chunkUploadId));
            }

            if (file.Length == 0)
            {
                throw new ArgumentNullException(nameof(file));
            }
            
            var uriBuilder = BuildUri(FilesChunkedContentMethod + "/" + EncodePathSegments(path));
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
            {
                Content = new StreamContent(file)
            };
            httpRequest.Headers.Add("X-Egnyte-Chunk-Num", chunkNumber.ToString());
            httpRequest.Headers.Add("X-Egnyte-Upload-Id", chunkUploadId);

            var serviceHandler = new ServiceHandler<CreateOrUpdateFileResponse>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return new ChunkUploadedMetadata(
                response.Headers.ContainsKey("x-egnyte-upload-id")
                    ? response.Headers["x-egnyte-upload-id"]
                    : string.Empty,
                response.Headers.ContainsKey("x-egnyte-chunk-num")
                    ? int.Parse(response.Headers["x-egnyte-chunk-num"])
                    : -1,
                response.Headers.ContainsKey("x-egnyte-chunk-sha512-checksum")
                    ? response.Headers["x-egnyte-chunk-sha512-checksum"]
                    : string.Empty);
        }

        /// <summary>
        /// Method for uploading last chunk of file. To upload previous chunks use
        /// ChunkedUploadFirstChunk or ChunkedUploadNextChunk
        /// </summary>
        /// <param name="path">Full path to file</param>
        /// <param name="chunkNumber">Chunk number. First should be 1, others 2, 3, etc.</param>
        /// <param name="chunkUploadId">Chunk upload id from response after uploading first chunk</param>
        /// <param name="file">Stream with part of file content</param>
        /// <returns>Uploaded file metadata</returns>
        public async Task<UploadedFileMetadata> ChunkedUploadLastChunk(
            string path,
            int chunkNumber,
            string chunkUploadId,
            MemoryStream file)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (chunkNumber <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(chunkNumber), "Chunk number must start with 1 and next chunks should be 2, 3, etc.");
            }

            if (string.IsNullOrWhiteSpace(chunkUploadId))
            {
                throw new ArgumentNullException(nameof(chunkUploadId));
            }

            if (file.Length == 0)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var uriBuilder = BuildUri(FilesChunkedContentMethod + "/" + EncodePathSegments(path));
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
            {
                Content = new StreamContent(file)
            };
            httpRequest.Headers.Add("X-Egnyte-Chunk-Num", chunkNumber.ToString());
            httpRequest.Headers.Add("X-Egnyte-Upload-Id", chunkUploadId);
            httpRequest.Headers.Add("X-Egnyte-Last-Chunk", "true");

            var serviceHandler = new ServiceHandler<CreateOrUpdateFileResponse>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return new UploadedFileMetadata(
                response.Headers.ContainsKey("x-sha512-checksum")
                    ? response.Headers["x-sha512-checksum"]
                    : response.Data.Checksum,
                response.Headers.ContainsKey("last-modified")
                    ? DateTime.Parse(response.Headers["last-modified"])
                    : DateTime.Now,
                response.Headers.ContainsKey("etag")
                    ? response.Headers["etag"]
                    : response.Data.EntryId);
        }

        /// <summary>
        /// Uses Folder Options API to modify folder options
        /// </summary>
        /// <param name="path">Full path to folder</param>
        /// <param name="folderDescription">Text description of the folder. Maximum 200 characters</param>
        /// <param name="publicLinks">Choose to allow public links from this folder for files and folders, files only, or not to allow public links.</param>
        /// <param name="restrictMoveDelete">Restricts move and delete operations to only Admins and Owners if true. This can be applied to /Shared and /Private top-level folders.</param>
        /// <param name="emailPreferences">JSON object with boolean keys that can modify periodic emails about file changes.</param>
        /// <param name="allowLinks">Choose whether links can be shared to files or sub-folders within this folder.</param>
        /// <returns></returns>
        public async Task<UpdateFolderMetadata> UpdateFolder(
            string path,
            string folderDescription = null,
            PublicLinksType? publicLinks = null,
            bool? restrictMoveDelete = null,
            string emailPreferences = null,
            bool? allowLinks = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(nameof(path));
            }

            if (folderDescription == null
                && !publicLinks.HasValue
                && restrictMoveDelete == null
                && emailPreferences == null
                && allowLinks == null)
            {
                throw new ArgumentException("None of the optional parameters were provided");
            }
            
            var uriBuilder = BuildUri(FilesMethod + "/" + EncodePathSegments(path));
            var httpRequest = new HttpRequestMessage(new HttpMethod("PATCH"), uriBuilder.Uri)
            {
                Content = new StringContent(
                    FilesHelper.MapFolderUpdateRequest(folderDescription, publicLinks, restrictMoveDelete, emailPreferences, allowLinks),
                    Encoding.UTF8,
                    "application/json")
            };

            var serviceHandler = new ServiceHandler<UpdateFolderResponse>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return FilesHelper.MapFolderUpdateToMetadata(response.Data);
        }

        /// <summary>
        /// Uses Metadata API to update the custom metadata for a given file or folder
        /// </summary>
        /// <param name="path">Full path to file or folder</param>
        /// <param name="sectionName">The custom metadata section name (aka namespace name)</param>
        /// <param name="properties">The custom metadata properties (aka namespace keys)</param>
        /// <returns></returns>
        public async Task<bool> UpdateFileOrFolderCustomMetadata(
            string path,
            string sectionName,
            FileOrFolderCustomMetadataProperties properties)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(nameof(path));
            }

            if (string.IsNullOrWhiteSpace(sectionName))
            {
                throw new ArgumentException(nameof(sectionName));
            }

            if (properties == null || properties.Count == 0)
            {
                throw new ArgumentException("None of the custom metadata properties were provided");
            }

            // retrieving file or folder metadata in order to get the entry id
            // since the method to update custom metadata only works using id (path not supported)
            var fileOrFolder = await ListFileOrFolder(path);

            var fileOrFolderType = fileOrFolder.IsFolder ? "folder" : "file";
            var groupOrEntryId = fileOrFolder.IsFolder ? fileOrFolder.AsFolder.FolderId : fileOrFolder.AsFile.GroupId;

            var uriBuilder = BuildUri(FilesMethod + "/ids/" + fileOrFolderType + "/" + Uri.EscapeDataString(groupOrEntryId) + "/properties/" + Uri.EscapeDataString(sectionName));
            var httpRequest = new HttpRequestMessage(new HttpMethod("PUT"), uriBuilder.Uri)
            {
                Content = new StringContent(
                    FilesHelper.MapUpdateFileOrFolderCustomMetadataRequest(properties),
                    Encoding.UTF8,
                    "application/json")
            };

            var serviceHandler = new ServiceHandler<string>(httpClient);
            await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return true;
        }

        DownloadedFile MapResponseToDownloadedFile(ServiceResponse<byte[]> response)
        {
            return new DownloadedFile(
                response.Data,
                response.Headers.ContainsKey("x-sha512-checksum")
                    ? response.Headers["x-sha512-checksum"]
                    : string.Empty,
                response.Headers.ContainsKey("last-modified")
                        ? DateTime.Parse(response.Headers["last-modified"])
                        : DateTime.Now,
                response.Headers.ContainsKey("etag")
                    ? response.Headers["etag"]
                    : string.Empty,
                response.Headers.ContainsKey("content-type")
                    ? response.Headers["content-type"]
                    : string.Empty,
                response.Headers.ContainsKey("content-length")
                        ? long.Parse(response.Headers["content-length"])
                        : 0,
                GetFullFileLengthFromRange(response.Headers));
        }

        DownloadedFileAsStream MapResponseToDownloadedFileAsStream(ServiceResponse<Stream> response)
        {
            return new DownloadedFileAsStream(
                response.Data,
                response.Headers.ContainsKey("x-sha512-checksum")
                    ? response.Headers["x-sha512-checksum"]
                    : string.Empty,
                response.Headers.ContainsKey("last-modified")
                    ? DateTime.Parse(response.Headers["last-modified"])
                    : DateTime.Now,
                response.Headers.ContainsKey("etag")
                    ? response.Headers["etag"]
                    : string.Empty,
                response.Headers.ContainsKey("content-type")
                    ? response.Headers["content-type"]
                    : string.Empty,
                response.Headers.ContainsKey("content-length")
                    ? long.Parse(response.Headers["content-length"])
                    : 0,
                GetFullFileLengthFromRange(response.Headers));
        }

        long GetFullFileLengthFromRange(Dictionary<string, string> headers)
        {
            if (headers.ContainsKey("content-range"))
            {
                var rangeParts = headers["content-range"].Split('/');
                if (rangeParts.Length == 2)
                {
                    return long.Parse(rangeParts[1]);
                }
            }

            return 0;
        }

        Uri PrepareListFileOrFolderUri(string path, bool listContent, bool allowedLinkTypes, bool? listCustomMetadata = null,
            int? count = 0, int? offset = 0, string sortBy = null, string sortDirection = null)
        {
            var query = new StringBuilder("list_content=" + listContent + "&allowed_link_types=" + allowedLinkTypes);

            if (listCustomMetadata != null)
            {
                query.Append("&list_custom_metadata=" + listCustomMetadata);
            }

            if (count != null)
            {
                query.Append("&count=" + count);
            }

            if (offset != null)
            {
                query.Append("&offset=" + offset);
            }

            if (sortBy != null)
            {
                query.Append("&sort_by=" + sortBy);
            }

            if (sortDirection != null)
            {
                query.Append("&sort_direction=" + sortDirection);
            }

            var uriBuilder = BuildUri(FilesMethod + "/" + EncodePathSegments(path), query.ToString());

            return uriBuilder.Uri;
        }

        Uri PrepareDownloadFileUri(string path, string entryId = "")
        {
            var query = string.Empty;

            if (!string.IsNullOrWhiteSpace(entryId))
            {
                query += "entry_id=" + entryId;
            }
            
            var uriBuilder = BuildUri(FilesContentMethod + "/" + EncodePathSegments(path), query);

            return uriBuilder.Uri;
        }
    }
}
