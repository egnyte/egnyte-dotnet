namespace Egnyte.Api.Files
{
    using System;

    public class UploadedFileMetadata
    {
        public UploadedFileMetadata(string checksum, DateTime lastModified, string entryId)
        {
            Checksum = checksum;
            LastModified = lastModified;
            EntryId = entryId;
        }

        /// <summary>
        /// SHA512 hash of entire file that can be used for validating upload integrity.
        /// </summary>
        public string Checksum { get; private set; }

        /// <summary>
        /// Indicates last modified date for file.
        /// </summary>
        public DateTime LastModified { get; private set; }

        /// <summary>
        /// Current value of the entity tag that can be used to compare whether two versions of a resource are the same.
        /// </summary>
        public string EntryId { get; private set; }
    }
}
