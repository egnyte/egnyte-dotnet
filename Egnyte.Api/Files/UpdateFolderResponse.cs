using Newtonsoft.Json;

namespace Egnyte.Api.Files
{
    public class UpdateFolderResponse
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "lastModified")]
        public long LastModified { get; set; }

        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "folder_id")]
        public string FolderId { get; set; }

        [JsonProperty(PropertyName = "folder_description")]
        public string FolderDescription { get; set; }

        [JsonProperty(PropertyName = "is_folder")]
        public bool IsFolder { get; set; }

        [JsonProperty(PropertyName = "public_links")]
        public string PublicLinks { get; set; }

        [JsonProperty(PropertyName = "restrict_move_delete")]
        public bool RestrictMoveDelete { get; set; }
    }
}
