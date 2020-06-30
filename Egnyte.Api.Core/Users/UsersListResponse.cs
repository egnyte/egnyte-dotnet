using Newtonsoft.Json;
using System.Collections.Generic;

namespace Egnyte.Api.Users
{
    internal class UserListResponse
    {
        [JsonProperty(PropertyName = "totalResults")]
        public int TotalResults { get; set; }

        [JsonProperty(PropertyName = "itemsPerPage")]
        public int ItemsPerPage { get; set; }

        [JsonProperty(PropertyName = "startIndex")]
        public int StartIndex { get; set; }

        [JsonProperty(PropertyName = "Resources")]
        public List<ExistingUserFlat> Resources { get; set; }
    }
}
