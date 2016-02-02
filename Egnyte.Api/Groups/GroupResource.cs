using Newtonsoft.Json;

namespace Egnyte.Api.Groups
{
    public class GroupResource
    {
        /// <summary>
        /// The globally unique group ID.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// The name of the group.
        /// </summary>
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }
    }
}
