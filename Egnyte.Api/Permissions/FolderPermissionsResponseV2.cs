using Newtonsoft.Json;
using System.Collections.Generic;
using Egnyte.Api.Common;

namespace Egnyte.Api.Permissions
{
    class FolderPermissionsResponseV2
    {
        [JsonProperty(PropertyName = "userPerms")]
        [JsonConverter(typeof(GroupOrUserPermissionsResponseV2Converter))]
        public List<GroupOrUserPermissionsResponse> Users { get; set; }

        [JsonProperty(PropertyName = "groupPerms")]
        [JsonConverter(typeof(GroupOrUserPermissionsResponseV2Converter))]
        public List<GroupOrUserPermissionsResponse> Groups { get; set; }

        [JsonProperty(PropertyName = "inheritsPermissions")]
        public bool InheritsPermissions { get; set; }
    }
}
