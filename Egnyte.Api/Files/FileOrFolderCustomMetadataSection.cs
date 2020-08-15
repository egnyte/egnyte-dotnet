using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Egnyte.Api.Files
{
    public class FileOrFolderCustomMetadataSection : Dictionary<string, FileOrFolderCustomMetadataProperties>
    {
        [JsonConstructor]
        internal FileOrFolderCustomMetadataSection()
        {
        }

        public FileOrFolderCustomMetadataProperties Properties => Values.FirstOrDefault() ?? new FileOrFolderCustomMetadataProperties();
    }
}
