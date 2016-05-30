namespace Egnyte.Api.Files
{
    using System;
    using System.Collections.Generic;

    public class FileMetadata : FileBasicMetadata
    {
        internal FileMetadata(
            string checksum,
            int size,
            string path,
            string name,
            bool locked,
            string entryId,
            string groupId,
            DateTime lastModified,
            string uploadedBy,
            int numberOfVersions,
            List<FileVersionMetadata> versions)
            : base(
                checksum,
                size,
                path,
                name,
                locked,
                entryId,
                groupId,
                lastModified,
                uploadedBy,
                numberOfVersions)
        {
            Versions = versions;
        }

        public List<FileVersionMetadata> Versions { get; private set; } 
    }
}
