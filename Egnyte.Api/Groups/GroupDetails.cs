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
        public List<string> Schemas { get; private set; }

        /// <summary>
        /// The globally unique group ID.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; private set; }

        /// <summary>
        /// The name of the group.
        /// </summary>
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; private set; }

        /// <summary>
        /// An array containing all users in the group.
        /// </summary>
        [JsonProperty(PropertyName = "members")]
        public List<GroupUser> Members { get; private set; }
    }
}
