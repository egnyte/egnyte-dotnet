using Newtonsoft.Json;
using System.Collections.Generic;

namespace Egnyte.Api.Permissions
{
    class FolderPermissionsResponse
    {
        [JsonProperty(PropertyName = "users")]
        public List<GroupOrUserPermissionsResponse> Users { get; set; }

        [JsonProperty(PropertyName = "groups")]
        public List<GroupOrUserPermissionsResponse> Groups { get; set; }
    }
}
