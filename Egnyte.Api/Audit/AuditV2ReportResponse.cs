using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Egnyte.Api.Audit
{
    public class AuditV2ReportResponse
    {
        [JsonProperty(PropertyName = "nextCursor")]
        public string NextCursor { get; set; }

        [JsonProperty(PropertyName = "events")]
        public List<AuditV2MetadataResponse> Events { get; set; }

        [JsonProperty(PropertyName = "moreEvents")]
        public Boolean MoreEvents { get; set; }
    }
}
