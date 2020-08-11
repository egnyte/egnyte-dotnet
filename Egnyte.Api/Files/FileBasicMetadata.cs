namespace Egnyte.Api.Files
{
    using System;

    public class FileBasicMetadata
    {
        internal FileBasicMetadata(
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
            int numberOfVersions)
        {
            Checksum = checksum;
            Size = size;
            Path = path;
            Name = name;
            Locked = locked;
            EntryId = entryId;
            GroupId = groupId;
            LastModified = lastModified;
            Uploaded = uploaded;
            UploadedBy = uploadedBy;
            NumberOfVersions = numberOfVersions;
        }

        public string Checksum { get; private set; }

        public long Size { get; private set; }

        public string Path { get; private set; }

        public string Name { get; private set; }

        public bool Locked { get; private set; }

        public string EntryId { get; private set; }

        public string GroupId { get; private set; }

        public DateTime LastModified { get; private set; }

        public DateTime Uploaded { get; private set; }

        public string UploadedBy { get; private set; }

        public int NumberOfVersions { get; private set; }
    }
}
