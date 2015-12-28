namespace Egnyte.Api.Files
{
    public class FileOrFolderMetadata
    {
        public FileOrFolderMetadata(
            bool isFolder,
            FolderExtendedMetadata foldesExtendedMetadata = null,
            FileMetadata fileMetadataResponse = null)
        {
            IsFolder = isFolder;
            AsFolder = foldesExtendedMetadata;
            AsFile = fileMetadataResponse;
        }

        public bool IsFolder { get; private set; }

        public FolderExtendedMetadata AsFolder { get; private set; }

        public FileMetadata AsFile { get; private set;  }
    }
}
