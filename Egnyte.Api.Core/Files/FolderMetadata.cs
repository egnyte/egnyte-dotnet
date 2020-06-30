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
            string[] allowedFolderLinkTypes)
        {
            Name = name;
            LastModified = lastModified;
            Path = path;
            FolderId = folderId;
            AllowedFileLinkTypes = allowedFileLinkTypes ?? new string[0];
            AllowedFolderLinkTypes = allowedFolderLinkTypes ?? new string[0];
        }

        public string Name { get; private set; }

        public DateTime LastModified { get; private set; }

        public string Path { get; private set; }

        public string FolderId { get; private set; }

        public string[] AllowedFileLinkTypes { get; private set; }

        public string[] AllowedFolderLinkTypes { get; private set; }
    }
}
