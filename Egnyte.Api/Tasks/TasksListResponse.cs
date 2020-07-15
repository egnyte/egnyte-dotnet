using System.Collections.Generic;
using Newtonsoft.Json;

namespace Egnyte.Api.Tasks
{
    public class TasksListResponse
    {
        [JsonProperty("results")]
        public List<TaskResponse> Tasks { get; set; }

        [JsonProperty("totalCount")]
        public int Count { get; set; }
    }
}