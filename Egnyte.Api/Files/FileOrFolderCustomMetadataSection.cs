using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Egnyte.Api.Files
{
    public class FileOrFolderCustomMetadataSection : Dictionary<string, Dictionary<string, object>>
    {
        [JsonConstructor]
        internal FileOrFolderCustomMetadataSection()
        {
        }

        public Dictionary<string, object> Properties => Values.FirstOrDefault() ?? new Dictionary<string, object>();
    }
}
