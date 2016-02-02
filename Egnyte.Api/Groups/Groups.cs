using Newtonsoft.Json;
using System.Collections.Generic;

namespace Egnyte.Api.Groups
{
    public class Groups
    {
        /// <summary>
        /// The SCIM schema version of the response.
        /// </summary>
        [JsonProperty(PropertyName = "schemas")]
        public List<string> Schemas { get; set; }

        /// <summary>
        /// The total number of results matching the query.
        /// </summary>
        [JsonProperty(PropertyName = "totalResults")]
        public int TotalResults { get; set; }

        /// <summary>
        /// The number of results returned.
        /// </summary>
        [JsonProperty(PropertyName = "itemsPerPage")]
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// The 1-based index of the first result in the current set of results.
        /// </summary>
        [JsonProperty(PropertyName = "startIndex")]
        public int StartIndex { get; set; }

        /// <summary>
        /// A JSON array that holds all of the group objects.
        /// </summary>
        [JsonProperty(PropertyName = "resources")]
        public List<GroupResource> Resources { get; set; }
    }
}
