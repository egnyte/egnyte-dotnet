using Newtonsoft.Json;
using System.Collections.Generic;

namespace Egnyte.Api.Links
{
    public class LinksList
    {
        [JsonProperty(PropertyName = "ids")]
        public List<string> Ids { get; private set; }

        [JsonProperty(PropertyName = "offset")]
        public int Offset { get; private set; }

        [JsonProperty(PropertyName = "count")]
        public int Count { get; private set; }

        [JsonProperty(PropertyName = "total_count")]
        public int TotalCount { get; private set; }
    }
}
