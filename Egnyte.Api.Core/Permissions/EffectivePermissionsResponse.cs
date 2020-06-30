using Newtonsoft.Json;

namespace Egnyte.Api.Permissions
{
    class EffectivePermissionsResponse
    {
        [JsonProperty(PropertyName = "permission")]
        public string Permission { get; set; }
    }
}
