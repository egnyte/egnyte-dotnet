namespace Egnyte.Api.Files
{
    using System;

    public class FileVersionMetadata
    {
        internal FileVersionMetadata(
            string checksum,
            int size,
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

        public int Size { get; private set; }

        public string EntryId { get; private set; }

        public DateTime LastModified { get; private set; }

        public string UploadedBy { get; private set; }
    }
}
