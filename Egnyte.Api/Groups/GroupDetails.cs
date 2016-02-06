using Newtonsoft.Json;
using System.Collections.Generic;

namespace Egnyte.Api.Groups
{
    public class GroupDetails
    {
        /// <summary>
        /// The SCIM schema version of the response.
        /// </summary>
        [JsonProperty(PropertyName = "schemas")]
        public List<string> Schemas { get; set; }

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

        /// <summary>
        /// An array containing all users in the group.
        /// </summary>
        [JsonProperty(PropertyName = "members")]
        public List<GroupUser> Members { get; set; }
    }
}
