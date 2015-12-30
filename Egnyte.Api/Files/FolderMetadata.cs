namespace Egnyte.Api.Files
{
    public class FolderMetadata
    {
        public FolderMetadata(
            string name,
            string path,
            string folderId,
            string[] allowedFileLinkTypes,
            string[] allowedFolderLinkTypes)
        {
            Name = name;
            Path = path;
            FolderId = folderId;
            AllowedFileLinkTypes = allowedFileLinkTypes ?? new string[0];
            AllowedFolderLinkTypes = allowedFolderLinkTypes ?? new string[0];
        }

        public string Name { get; private set; }

        public string Path { get; private set; }

        public string FolderId { get; private set; }

        public string[] AllowedFileLinkTypes { get; private set; }

        public string[] AllowedFolderLinkTypes { get; private set; }
    }
}
