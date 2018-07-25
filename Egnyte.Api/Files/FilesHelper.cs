namespace Egnyte.Api.Files
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

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
                        response.AllowedFileLinkTypes,
                        response.AllowedFolderLinkTypes,
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

        internal static string MapFolderUpdateRequest(
            string folderDescription = null,
            PublicLinksType? publicLinks = null,
            bool? restrictMoveDelete = null,
            string emailPreferences = null)
        {
            var jsonParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(folderDescription))
            {
                jsonParams.Add("\"folder_description\" : \"" + folderDescription + "\"");
            }
            if (publicLinks.HasValue)
            {
                jsonParams.Add("\"public_links\" : \"" + MapPublicLinksType(publicLinks.Value) + "\"");
            }
            if (restrictMoveDelete != null)
            {
                jsonParams.Add("\"restrict_move_delete\" : \"" + restrictMoveDelete.Value + "\"");
            }
            if (!string.IsNullOrWhiteSpace(emailPreferences))
            {
                jsonParams.Add("\"email_preferences\" : \"" + emailPreferences + "\"");
            }

            var builder = new StringBuilder();
            builder.Append("{");
            foreach (var param in jsonParams)
            {
                builder.Append(param);
            }

            builder.Append("}");

            return builder.ToString();
        }

        private static List<FileBasicMetadata> MapChildFilesResponse(IEnumerable<FileMetadataResponse> files)
        {
            if (files == null)
            {
                return new List<FileBasicMetadata>();
            }

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
            if (folders == null)
            {
                return new List<FolderMetadata>();
            }

            return folders.Select(
                f => new FolderMetadata(
                    f.Name,
                    f.Path,
                    f.FolderId,
                    f.AllowedFileLinkTypes,
                    f.AllowedFolderLinkTypes))
                    .ToList();
        }

        private static List<FileVersionMetadata> MapFileVersions(IEnumerable<FileVersionMetadataResponse> versions)
        {
            if (versions == null)
            {
                return new List<FileVersionMetadata>();
            }

            return
                versions.Select(
                    v => new FileVersionMetadata(v.Checksum, v.Size, v.EntryId, v.LastModified, v.UploadedBy))
                    .ToList();
        }

        private static string MapPublicLinksType(PublicLinksType type)
        {
            switch (type)
            {
                case PublicLinksType.FilesFolders:
                    return "files_folders";
                case PublicLinksType.Files:
                    return "files";
                default:
                    return "disabled";
            }
        }
    }
}
