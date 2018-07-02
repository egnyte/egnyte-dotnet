using Newtonsoft.Json;
using System.Collections.Generic;

namespace Egnyte.Api.Links
{
    internal class LinksListV2Response
    {
        [JsonProperty("links")]
        public List<LinkDetailsV2Response> Links { get; private set; }

        [JsonProperty("count")]
        public int Count { get; private set; }
    }
}
