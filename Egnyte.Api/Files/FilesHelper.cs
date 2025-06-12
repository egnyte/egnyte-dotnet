namespace Egnyte.Api.Files
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class FilesHelper
    {
        internal static FileOrFolderMetadata MapResponseToMetadata(ListFileOrFolderResponse response)
        {
            var customMetadata = new FileOrFolderCustomMetadata();
            if (response.CustomMetadata != null)
            {
                customMetadata.AddRange(response.CustomMetadata);
            }

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
                        response.FolderDescription,
                        response.TotalCount,
                        ConvertFromUnixTimestamp(response.LastModifiedFolder),
                        response.RestrictMoveDelete,
                        response.PublicLinks,
                        response.AllowedFileLinkTypes,
                        response.AllowedFolderLinkTypes,
                        MapChildFoldersResponse(response.Folders),
                        MapChildFilesResponse(response.Files),
                        customMetadata));
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
                    response.LastModifiedFile,
                    response.Uploaded,
                    response.UploadedBy,
                    response.NumberOfVersions,
                    MapFileVersions(response.Versions),
                    customMetadata));
        }

        internal static string MapFolderUpdateRequest(
            string folderDescription = null,
            PublicLinksType? publicLinks = null,
            bool? restrictMoveDelete = null,
            string emailPreferences = null,
            bool? allowLinks = null)
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
                jsonParams.Add("\"restrict_move_delete\" : " + (restrictMoveDelete.Value ? "true" : "false"));
            }
            if (!string.IsNullOrWhiteSpace(emailPreferences))
            {
                jsonParams.Add("\"email_preferences\" : " + emailPreferences);
            }
            if (allowLinks != null)
            {
                jsonParams.Add("\"allow_links\" : " + (allowLinks.Value ? "true" : "false"));
            }

            var content = "{" + string.Join(",", jsonParams) + "}";

            return content;
        }

        internal static string MapUpdateFileOrFolderCustomMetadataRequest(FileOrFolderCustomMetadataProperties properties)
        {
            var jsonParams = new List<string>();

            foreach (var kvp in properties)
            {
                jsonParams.Add("\"" + kvp.Key + "\" : \"" + kvp.Value + "\"");
            }

            var content = "{" + string.Join(",", jsonParams) + "}";

            return content;
        }

        internal static UpdateFolderMetadata MapFolderUpdateToMetadata(UpdateFolderResponse response)
        {
            return new UpdateFolderMetadata
            {
                Name = response.Name,
                Path = response.Path,
                FolderDescription = response.FolderDescription,
                LastModified = ConvertFromUnixTimestamp(response.LastModified),
                IsFolder = response.IsFolder,
                FolderId = response.FolderId,
                PublicLinks = ParsePublicLinksType(response.PublicLinks),
                RestrictMoveDelete = response.RestrictMoveDelete,
                AllowLinks = response.AllowLinks
            };
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
                    f.Uploaded,
                    f.UploadedBy,
                    f.NumberOfVersions,
                    f.CustomMetadata)).ToList();
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
                    ConvertFromUnixTimestamp(f.LastModified),
                    f.Path,
                    f.FolderId,
                    f.AllowedFileLinkTypes,
                    f.AllowedFolderLinkTypes,
                    f.CustomMetadata))
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
        private static PublicLinksType ParsePublicLinksType(string publicLink)
        {
            switch (publicLink)
            {
                case "files_folders": return PublicLinksType.FilesFolders;
                case "files": return PublicLinksType.Files;
                default: return PublicLinksType.Disabled;
            }
        }

        private static DateTime ConvertFromUnixTimestamp(long timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddMilliseconds(timestamp)
                .ToLocalTime();
        }
    }
}
