namespace Egnyte.Api.Files
{
    using System;

    using Newtonsoft.Json;

    internal class FileVersionMetadataResponse
    {
        [JsonProperty(PropertyName = "checksum")]
        public string Checksum { get; set; }

        [JsonProperty(PropertyName = "size")]
        public int Size { get; set; }

        [JsonProperty(PropertyName = "is_folder")]
        public bool IsFolder { get; set; }

        [JsonProperty(PropertyName = "entry_id")]
        public string EntryId { get; set; }

        [JsonProperty(PropertyName = "last_modified")]
        public DateTime LastModified { get; set; }

        [JsonProperty(PropertyName = "uploaded_by")]
        public string UploadedBy { get; set; }
    }
}
