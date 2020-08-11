using Egnyte.Api.Common;

namespace Egnyte.Api.Files
{
    using System;

    using Newtonsoft.Json;

    internal class FileMetadataResponse
    {
        [JsonProperty(PropertyName = "checksum")]
        public string Checksum { get; set; }

        [JsonProperty(PropertyName = "size")]
        public long Size { get; set; }

        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "is_folder")]
        public bool IsFolder { get; set; }

        [JsonProperty(PropertyName = "locked")]
        public bool Locked { get; set; }

        [JsonProperty(PropertyName = "entry_id")]
        public string EntryId { get; set; }

        [JsonProperty(PropertyName = "group_id")]
        public string GroupId { get; set; }

        [JsonProperty(PropertyName = "last_modified")]
        public DateTime LastModified { get; set; }

        [JsonProperty(PropertyName = "uploaded")]
        [JsonConverter(typeof(UnixTimeConverter))]
        public DateTime Uploaded { get; set; }

        [JsonProperty(PropertyName = "uploaded_by")]
        public string UploadedBy { get; set; }

        [JsonProperty(PropertyName = "num_versions")]
        public int NumberOfVersions { get; set; }
    }
}
