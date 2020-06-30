using Newtonsoft.Json;

namespace Egnyte.Api.Audit
{
    class CreateLoginAuditReportResponse
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}
