using Newtonsoft.Json;
using System;

namespace Egnyte.Api.ProjectFolders
{
    public class ProjectDetails
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "rootFolderId")]
        public string RootFolderId { get; set; }
        
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "projectId")]
        public string ProjectId { get; set; }

        [JsonProperty(PropertyName = "customerName")]
        public string CustomerName { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "location")]
        public ProjectFolderLocation Location { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "startDate")]
        public DateTime StartDate { get; set; }
        
        [JsonProperty(PropertyName = "createdBy")]
        public long CreatedBy { get; set; }
        
        [JsonProperty(PropertyName = "lastUpdatedBy")]
        public long LastUpdatedBy { get; set; }
        
        [JsonProperty(PropertyName = "creationTime")]
        public DateTime CreationTime { get; set; }
        
        [JsonProperty(PropertyName = "lastModifiedTime")]
        public DateTime LastModifiedTime { get; set; }
    }
}
