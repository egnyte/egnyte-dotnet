namespace Egnyte.Api.Tasks
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class TaskResponse
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "task")]
        public string Task { get; set; }

        [JsonProperty(PropertyName = "creationDate")]
        public long CreationDateTimestamp { get; set; }

        [JsonProperty(PropertyName = "completionDate")]
        public long? CompletionDateTimestamp { get; set; }

        [JsonProperty(PropertyName = "dueDate")]
        public DateTime? DueDate { get; set; }

        [JsonProperty(PropertyName = "dueDateTimestamp")]
        public long? DueDateTimestamp { get; set; }

        [JsonProperty(PropertyName = "assignor")]
        public TaskUserResponse Assignor { get; set; }

        [JsonProperty(PropertyName = "assignees")]
        public List<TaskUserResponse> Assignees { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "file")]
        public TaskFile File { get; set; }
    }
}