using Newtonsoft.Json;
using System;

namespace Egnyte.Api.Links
{
    internal class LinkDetailsV2Response : LinkDetailsResponse
    {
        [JsonProperty(PropertyName = "resource_id")]
        public string ResourceId { get; set; }

        [JsonProperty(PropertyName = "expiry_clicks")]
        public int ExpiryClicks { get; set; }

        [JsonProperty(PropertyName = "expiry_date")]
        public DateTime ExpiryDate { get; set; }   
    }
}
