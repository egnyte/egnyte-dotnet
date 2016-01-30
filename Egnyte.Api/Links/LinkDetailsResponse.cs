using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Egnyte.Api.Links
{
    internal class LinkDetailsResponse
    {
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }
        
        [JsonProperty(PropertyName = "type")]
        public string LinkType { get; set; }
        
        [JsonProperty(PropertyName = "accessibility")]
        public string Accessibility { get; set; }

        [JsonProperty(PropertyName = "protection")]
        public string Protection { get; set; }

        [JsonProperty(PropertyName = "recipients")]
        public List<string> Recipients { get; set; }

        [JsonProperty(PropertyName = "notify")]
        public bool Notify { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "link_to_current")]
        public bool LinkToCurrent { get; set; }

        [JsonProperty(PropertyName = "creation_date")]
        public DateTime CreationDate { get; set; }

        [JsonProperty(PropertyName = "created_by")]
        public string CreatedBy { get; set; }   
    }
}
