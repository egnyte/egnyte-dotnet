using System;
using System.Collections.Generic;

namespace Egnyte.Api.Links
{
    public class LinkDetails
    {
        internal LinkDetails(
            string path,
            LinkType type,
            LinkAccessibility accessibility,
            bool notify,
            bool linkToCurrent,
            DateTime creationDate,
            string createdBy,
            ProtectionType protection,
            List<string> recipients,
            string url,
            string id)
        {
            Path = path;
            Type = type;
            Accessibility = accessibility;
            Notify = notify;
            LinkToCurrent = linkToCurrent;
            CreationDate = creationDate;
            CreatedBy = createdBy;
            Protection = protection;
            Recipients = recipients;
            Url = url;
            Id = id;
        }

        /// <summary>
        /// The absolute path of the target resource, either file or folder.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// The type of link, either file, folder, or upload
        /// </summary>
        public LinkType Type { get; private set; }

        /// <summary>
        /// Who a link is accessible by, one of anyone, password, domain, or recipients.
        /// </summary>
        public LinkAccessibility Accessibility { get; private set; }

        /// <summary>
        /// Boolean as to whether the link creator will be notified via email when link is accessed.
        /// </summary>
        public bool Notify { get; private set; }

        /// <summary>
        /// Boolean as to whether the link will always refer to the current version of file. Only applicable for file links.
        /// </summary>
        public bool LinkToCurrent { get; private set; }

        /// <summary>
        /// Date and time of creation of the link
        /// </summary>
        public DateTime CreationDate { get; private set; }

        /// <summary>
        /// Username of the user that created the link
        /// </summary>
        public string CreatedBy { get; private set; }

        /// <summary>
        /// If "PREVIEW" for a file link, it is a a preview-only link to the file, otherwise it is "NONE"
        /// </summary>
        public ProtectionType Protection { get; private set; }

        /// <summary>
        /// An array of valid email addresses to which the link was sent.
        /// </summary>
        public List<string> Recipients { get; private set; }

        /// <summary>
        /// The full url of the link
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// The id of the link
        /// </summary>
        public string Id { get; private set; }
    }
}
