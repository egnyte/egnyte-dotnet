using System.Linq;

namespace Egnyte.Api.Links
{
    public static class LinksHelper
    {
        internal static LinksListV2 MapLinksListResponse(LinksListV2Response data)
        {
            return new LinksListV2(
                data.Links.Select(l => MapGetLinkDetailsResponse(l)).ToList(),
                data.Count);
        }

        internal static LinkDetails MapGetLinkDetailsResponse(LinkDetailsResponse data)
        {
            return new LinkDetails(
                data.Path,
                ParseLinkType(data.LinkType),
                ParseAccessibility(data.Accessibility),
                data.Notify,
                data.LinkToCurrent,
                data.CreationDate,
                data.CreatedBy,
                ParseProtectionType(data.Protection),
                data.Recipients,
                data.Url,
                data.Id);
        }

        internal static LinkDetailsV2 MapGetLinkDetailsResponse(LinkDetailsV2Response data)
        {
            return new LinkDetailsV2(
                data.Path,
                ParseLinkType(data.LinkType),
                ParseAccessibility(data.Accessibility),
                data.Notify,
                data.LinkToCurrent,
                data.CreationDate,
                data.CreatedBy,
                ParseProtectionType(data.Protection),
                data.Recipients,
                data.Url,
                data.Id,
                data.ResourceId,
                data.ExpiryClicks,
                data.ExpiryDate);
        }

        internal static CreatedLink MapFlatCreatedLinkToCreatedLink(CreatedLinkResponse data)
        {
            return new CreatedLink(
                data.Links,
                data.Path,
                ParseLinkType(data.LinkType),
                ParseAccessibility(data.Accessibility),
                data.Notify,
                data.LinkToCurrent,
                data.ExpiryDate,
                data.CreationDate,
                data.CreatedBy);
        }

        private static ProtectionType ParseProtectionType(string protection)
        {
            switch (protection.ToLower())
            {
                case "preview":
                    return ProtectionType.Preview;
                case "preview_donwload":
                    return ProtectionType.PreviewDownload;
                default:
                    return ProtectionType.None;
            }
        }

        private static LinkAccessibility ParseAccessibility(string accessibility)
        {
            if (string.IsNullOrEmpty(accessibility))
                return LinkAccessibility.Anyone;

            switch (accessibility.ToLower())
            {
                case "domain":
                    return LinkAccessibility.Domain;
                case "password":
                    return LinkAccessibility.Password;
                case "recipients":
                    return LinkAccessibility.Recipients;
                default:
                    return LinkAccessibility.Anyone;
            }
        }

        private static LinkType ParseLinkType(string linkType)
        {
            switch (linkType)
            {
                case "file": return LinkType.File;
                case "upload": return LinkType.Upload;
                default: return LinkType.Folder;
            }
        }
    }
}
