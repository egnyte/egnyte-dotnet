using Newtonsoft.Json;

namespace Egnyte.Api.Groups
{
    public class GroupUser
    {
        /// <summary>
        /// The username of a group member.
        /// </summary>
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        /// <summary>
        /// The globally unique id of a group member.
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public long Value { get; set; }

        /// <summary>
        /// The display name of a group member.
        /// </summary>
        [JsonProperty(PropertyName = "display")]
        public string Display { get; set; }
    }
}
