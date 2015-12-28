namespace Egnyte.Api.Files
{
    using System.Collections.Generic;
    using System.Linq;

    public static class FilesHelper
    {
        internal static FileOrFolderMetadata MapResponseToMetadata(ListFileOrFolderResponse response)
        {
            if (response.IsFolder)
            {
                return new FileOrFolderMetadata(
                    true,
                    new FolderExtendedMetadata(
                        response.Name,
                        response.Count,
                        response.Offset,
                        response.Path,
                        response.FolderId,
                        response.TotalCount,
                        response.RestrictMoveDelete,
                        response.PublicLinks,
                        MapChildFoldersResponse(response.Folders),
                        MapChildFilesResponse(response.Files)));
            }

            return new FileOrFolderMetadata(
                false,
                null,
                new FileMetadata(
                    response.Checksum,
                    response.Size,
                    response.Path,
                    response.Name,
                    response.Locked,
                    response.EntryId,
                    response.GroupId,
                    response.LastModified,
                    response.UploadedBy,
                    response.NumberOfVersions,
                    MapFileVersions(response.Versions)));
        }

        private static List<FileBasicMetadata> MapChildFilesResponse(IEnumerable<FileMetadataResponse> files)
        {
            return files.Select(
                f => new FileBasicMetadata(
                    f.Checksum,
                    f.Size,
                    f.Path,
                    f.Name,
                    f.Locked,
                    f.EntryId,
                    f.GroupId,
                    f.LastModified,
                    f.UploadedBy,
                    f.NumberOfVersions)).ToList();
        }

        private static List<FolderMetadata> MapChildFoldersResponse(IEnumerable<FolderMetadataResponse> folders)
        {
            return folders.Select(f => new FolderMetadata(f.Name, f.Path, f.FolderId)).ToList();
        }

        private static List<FileVersionMetadata> MapFileVersions(IEnumerable<FileVersionMetadataResponse> versions)
        {
            return
                versions.Select(
                    v => new FileVersionMetadata(v.Checksum, v.Size, v.EntryId, v.LastModified, v.UploadedBy))
                    .ToList();
        }
    }
}
