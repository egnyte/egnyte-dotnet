using Newtonsoft.Json;
using System.Collections.Generic;

namespace Egnyte.Api.Links
{
    public class Link
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; private set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; private set; }

        [JsonProperty(PropertyName = "recipients")]
        public List<string> Recipients { get; private set; }
    }
}
