namespace Egnyte.Api.Files
{
    using System;
    using System.Collections.Generic;

    public class FileMetadata : FileBasicMetadata
    {
        internal FileMetadata(
            string checksum,
            long size,
            string path,
            string name,
            bool locked,
            string entryId,
            string groupId,
            DateTime lastModified,
            DateTime uploaded,
            string uploadedBy,
            int numberOfVersions,
            List<FileVersionMetadata> versions,
            FileOrFolderCustomMetadata customMetadata)
            : base(
                checksum,
                size,
                path,
                name,
                locked,
                entryId,
                groupId,
                lastModified,
                uploaded,
                uploadedBy,
                numberOfVersions,
                customMetadata)
        {
            Versions = versions;
        }

        public List<FileVersionMetadata> Versions { get; private set; } 
    }
}
