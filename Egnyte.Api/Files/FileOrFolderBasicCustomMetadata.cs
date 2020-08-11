using System.Collections.Generic;
using System.Linq;

namespace Egnyte.Api.Files
{
    public class FileOrFolderBasicCustomMetadata<T> : List<T> where T : FileOrFolderCustomMetadataSection
    {
        internal FileOrFolderBasicCustomMetadata()
        {
        }

        public T this[string sectionName] => this.FirstOrDefault(s => s.ContainsKey(sectionName));
    }
}
