using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Egnyte.Api.Links
{
    internal class CreatedLinkResponse
    {
        [JsonProperty(PropertyName = "links")]
        public List<Link> Links { get; set; }

        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string LinkType { get; set; }

        [JsonProperty(PropertyName = "accessibility")]
        public string Accessibility { get; set; }

        [JsonProperty(PropertyName = "notify")]
        public bool Notify { get; set; }

        [JsonProperty(PropertyName = "link_to_current")]
        public bool LinkToCurrent { get; set; }

        [JsonProperty(PropertyName = "expiry_date")]
        public DateTime ExpiryDate { get; set; }

        [JsonProperty(PropertyName = "creation_date")]
        public DateTime CreationDate { get; set; }

        [JsonProperty(PropertyName = "created_by")]
        public string CreatedBy { get; set; }
    }
}
