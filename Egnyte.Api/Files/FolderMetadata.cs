namespace Egnyte.Api.Files
{
    public class FolderMetadata
    {
        public FolderMetadata(
            string name,
            string path,
            string folderId)
        {
            Name = name;
            Path = path;
            FolderId = folderId;
        }

        public string Name { get; private set; }

        public string Path { get; private set; }

        public string FolderId { get; private set; }
    }
}
