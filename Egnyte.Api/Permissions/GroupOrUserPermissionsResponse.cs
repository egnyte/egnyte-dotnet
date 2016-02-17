using Newtonsoft.Json;

namespace Egnyte.Api.Permissions
{
    class GroupOrUserPermissionsResponse
    {
        [JsonProperty(PropertyName = "subject")]
        public string Subject { get; set; }

        [JsonProperty(PropertyName = "permission")]
        public string Permission { get; set; }
    }
}
