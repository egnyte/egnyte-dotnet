using System;

namespace Egnyte.Api.Files
{
    public class UpdateFolderMetadata
    {
        public string Name { get; set; }
        
        public DateTime LastModified { get; set; }
        
        public string Path { get; set; }

        public string FolderId { get; set; }
        
        public string FolderDescription { get; set; }
        
        public bool IsFolder { get; set; }
        
        public PublicLinksType PublicLinks { get; set; }
        
        public bool RestrictMoveDelete { get; set; }
    }
}
