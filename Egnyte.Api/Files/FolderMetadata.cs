namespace Egnyte.Api.Files
{
    using System;

    public class FolderMetadata
    {
        internal FolderMetadata(
            string name,
            DateTime lastModified,
            string path,
            string folderId,
            string[] allowedFileLinkTypes,
            string[] allowedFolderLinkTypes,
            FileOrFolderCustomMetadata customMetadata)
        {
            Name = name;
            LastModified = lastModified;
            Path = path;
            FolderId = folderId;
            AllowedFileLinkTypes = allowedFileLinkTypes ?? new string[0];
            AllowedFolderLinkTypes = allowedFolderLinkTypes ?? new string[0];
            CustomMetadata = customMetadata ?? new FileOrFolderCustomMetadata();
        }

        public string Name { get; private set; }

        public DateTime LastModified { get; private set; }

        public string Path { get; private set; }

        public string FolderId { get; private set; }

        public string[] AllowedFileLinkTypes { get; private set; }

        public string[] AllowedFolderLinkTypes { get; private set; }

        public FileOrFolderCustomMetadata CustomMetadata { get; private set; }
    }
}
