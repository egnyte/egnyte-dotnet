using Newtonsoft.Json;

namespace Egnyte.Api.ProjectFolders
{
    public class CreateFromTemplateResponse
    {
        public class GroupCreated
        {
            [JsonProperty(PropertyName = "id")]
            public string Id { get; set; }

            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }
        }

        [JsonProperty(PropertyName = "groupsCreated")]
        public GroupCreated[] GroupsCreated { get; set; }

    }
}
