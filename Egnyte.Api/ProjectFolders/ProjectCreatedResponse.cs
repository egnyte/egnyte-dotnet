using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Egnyte.Api.ProjectFolders
{
    internal class ProjectCreatedResponse
    {
        [JsonProperty(PropertyName = "id")]
        public string ProjectId { get; set; }
    }
}
