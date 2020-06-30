namespace Egnyte.Api.Files
{
    using Newtonsoft.Json;

    public class CreateOrUpdateFileResponse
    {
        [JsonProperty(PropertyName = "checksum")]
        public string Checksum { get; set; }

        [JsonProperty(PropertyName = "group_id")]
        public string GroupId { get; set; }

        [JsonProperty(PropertyName = "entry_id")]
        public string EntryId { get; set; }
    }
}
