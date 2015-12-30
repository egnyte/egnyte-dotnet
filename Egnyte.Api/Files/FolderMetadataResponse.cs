namespace Egnyte.Api.Files
{
    using Newtonsoft.Json;

    public class FolderMetadataResponse
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "folder_id")]
        public string FolderId { get; set; }

        [JsonProperty(PropertyName = "is_folder")]
        public bool IsFolder { get; set; }

        [JsonProperty(PropertyName = "allowed_file_link_types")]
        public string[] AllowedFileLinkTypes { get; set; }

        [JsonProperty(PropertyName = "allowed_folder_link_types")]
        public string[] AllowedFolderLinkTypes { get; set; }
    }
}
