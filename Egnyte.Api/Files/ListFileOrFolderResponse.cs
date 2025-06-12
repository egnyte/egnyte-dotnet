using Egnyte.Api.Common;

namespace Egnyte.Api.Files
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    internal class ListFileOrFolderResponse
    {
        [JsonProperty(PropertyName = "is_folder")]
        public bool IsFolder { get; set; }

        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        #region Folder response

        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }

        [JsonProperty(PropertyName = "offset")]
        public int Offset { get; set; }

        [JsonProperty(PropertyName = "folder_id")]
        public string FolderId { get; set; }

        [JsonProperty(PropertyName = "folder_description")]
        public string FolderDescription { get; set; }

        [JsonProperty(PropertyName = "total_count")]
        public int TotalCount { get; set; }

        [JsonProperty(PropertyName = "restrict_move_delete")]
        public bool RestrictMoveDelete { get; set; }

        [JsonProperty(PropertyName = "public_links")]
        public string PublicLinks { get; set; }

        [JsonProperty(PropertyName = "lastModified")]
        public long LastModifiedFolder { get; set; }

        [JsonProperty(PropertyName = "allowed_file_link_types")]
        public string[] AllowedFileLinkTypes { get; set; }

        [JsonProperty(PropertyName = "allowed_folder_link_types")]
        public string[] AllowedFolderLinkTypes { get; set; }

        [JsonProperty(PropertyName = "folders")]
        public List<FolderMetadataResponse> Folders { get; set; }

        [JsonProperty(PropertyName = "files")]
        public List<FileMetadataResponse> Files { get; set; }

        #endregion

        #region File response

        [JsonProperty(PropertyName = "checksum")]
        public string Checksum { get; set; }

        [JsonProperty(PropertyName = "size")]
        public long Size { get; set; }

        [JsonProperty(PropertyName = "locked")]
        public bool Locked { get; set; }

        [JsonProperty(PropertyName = "entry_id")]
        public string EntryId { get; set; }

        [JsonProperty(PropertyName = "group_id")]
        public string GroupId { get; set; }

        [JsonProperty(PropertyName = "last_modified")]
        public DateTime LastModifiedFile { get; set; }

        [JsonProperty(PropertyName = "uploaded")]
        [JsonConverter(typeof(UnixTimeConverter))]
        public DateTime Uploaded { get; set; }

        [JsonProperty(PropertyName = "uploaded_by")]
        public string UploadedBy { get; set; }

        [JsonProperty(PropertyName = "num_versions")]
        public int NumberOfVersions { get; set; }

        [JsonProperty(PropertyName = "versions")]
        public List<FileVersionMetadataResponse> Versions { get; set; }

        [JsonProperty(PropertyName = "custom_metadata")]
        public FileOrFolderCustomMetadataResponse CustomMetadata { get; set; }

        #endregion
    }
}
