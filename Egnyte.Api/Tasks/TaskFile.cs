namespace Egnyte.Api.Tasks
{
    using Egnyte.Api.Users;
    using Newtonsoft.Json;

    public class TaskFile
    {
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "parentPath")]
        public string ParentPath { get; set; }

        [JsonProperty(PropertyName = "entryId")]
        public string EntryId { get; set; }

        [JsonProperty(PropertyName = "groupId")]
        public string GroupId { get; set; }

        [JsonProperty(PropertyName = "folderId")]
        public string FolderId { get; set; }

        [JsonProperty(PropertyName = "size")]
        public long Size { get; set; }
    }
}