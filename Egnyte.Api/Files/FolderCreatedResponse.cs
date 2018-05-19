using Newtonsoft.Json;

namespace Egnyte.Api.Files
{
    public class FolderCreatedResponse
    {
        [JsonProperty(PropertyName = "folder_id")]
        public string FolderId { get; set; }
    }
}
