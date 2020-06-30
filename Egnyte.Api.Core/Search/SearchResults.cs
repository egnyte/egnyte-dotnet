using Newtonsoft.Json;
using System.Collections.Generic;

namespace Egnyte.Api.Search
{
    public class SearchResults
    {
        /// <summary>
        /// List that holds all of the matched content
        /// </summary>
        [JsonProperty(PropertyName = "results")]
        public List<SearchResultItem> Results { get; private set; }

        /// <summary>
        /// The total number of results matching the query
        /// </summary>
        [JsonProperty(PropertyName = "total_count")]
        public int TotalCount { get; private set; }

        /// <summary>
        /// The 0-based index of the first result in the current set of results
        /// </summary>
        [JsonProperty(PropertyName = "offset")]
        public int Offset { get; private set; }

        /// <summary>
        /// The number of results returned
        /// </summary>
        [JsonProperty(PropertyName = "count")]
        public int Count { get; private set; }
    }
}
