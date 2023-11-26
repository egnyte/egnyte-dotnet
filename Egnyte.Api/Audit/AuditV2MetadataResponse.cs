using Newtonsoft.Json;
using System;

namespace Egnyte.Api.Audit
{
    public class AuditV2MetadataResponse
    {
        [JsonProperty(PropertyName = "date")]
        public Int64 Date { get; set; }

        [JsonProperty(PropertyName = "sourcePath")]
        public string SourcePath { get; set; }

        [JsonProperty(PropertyName = "targetPath")]
        public string TargetPath { get; set; }

        [JsonProperty(PropertyName = "user")]
        public string User { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }

        [JsonProperty(PropertyName = "access")]
        public string Access { get; set; }

        [JsonProperty(PropertyName = "ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty(PropertyName = "actionInfo")]
        public string ActionInfo { get; set; }

        [JsonProperty(PropertyName = "auditSource")]
        public string AuditSource { get; set; }

    }
}
