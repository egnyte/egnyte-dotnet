using Newtonsoft.Json;
using System.Collections.Generic;

namespace Egnyte.Api.Links
{
    public class LinksList
    {
        [JsonProperty(PropertyName = "ids")]
        public List<string> Ids { get; set; }

        [JsonProperty(PropertyName = "offset")]
        public int Offset { get; set; }

        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }

        [JsonProperty(PropertyName = "total_count")]
        public int TotalCount { get; set; }
    }
}
