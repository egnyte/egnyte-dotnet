using Newtonsoft.Json;

namespace Egnyte.Api.ProjectFolders
{
    public class ProjectFolderLocation
    {
        [JsonProperty(PropertyName = "streetAddress1")]
        public string StreetAddress1 { get; set; }

        [JsonProperty(PropertyName = "streetAddress2")]
        public string StreetAddress2 { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }
    }
}
