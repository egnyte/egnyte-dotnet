namespace Egnyte.Api.Files
{
    using System;

    public class FileVersionMetadata
    {
        internal FileVersionMetadata(
            string checksum,
            long size,
            string entryId,
            DateTime lastModified,
            string uploadedBy)
        {
            Checksum = checksum;
            Size = size;
            EntryId = entryId;
            LastModified = lastModified;
            UploadedBy = uploadedBy;
        }

        public string Checksum { get; private set; }

        public long Size { get; private set; }

        public string EntryId { get; private set; }

        public DateTime LastModified { get; private set; }

        public string UploadedBy { get; private set; }
    }
}
