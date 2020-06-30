using System;
using System.Collections.Generic;

namespace Egnyte.Api.Links
{
    public class NewLink
    {
        /// <summary>
        /// Required. The absolute path of the target file or folder.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Required. This determines what type of link will be created.
        /// </summary>
        public LinkType Type { get; set; }

        /// <summary>
        /// Required (except upload links). Determines who a link is accessible by.
        /// </summary>
        public LinkAccessibility Accessibility { get; set; }

        /// <summary>
        /// Optional. When accesibility is set to password, then password can be provided.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Optional. If true is set, link will be sent via email by Egnyte.
        /// </summary>
        public bool? SendEmail { get; set; }

        /// <summary>
        /// Optional. List email addresses of recipients of the link. Only required if send_email is True.
        /// </summary>
        public List<string> Recipients { get; set; }

        /// <summary>
        /// Optional. Personal message to be sent in link email. Only applies if send_email is True.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Optional. If True is set, a copy of the link message will be sent to the link creator.
        /// Only applies if send_email is True. Defaults to False.
        /// </summary>
        public bool? CopyMe { get; set; }

        /// <summary>
        /// Optional. If True is set, link creator will be notified via email when link is accessed.
        /// </summary>
        public bool? Notify { get; set; }

        /// <summary>
        /// Optional. If True is set, link will always refer to current version of file.
        /// Only applicable for file links.
        /// </summary>
        public bool? LinkToCurrent { get; set; }

        /// <summary>
        /// Optional. The expiry date for the link. If expiry_date is specified,
        /// expiry_clicks cannot be set.
        /// </summary>
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// Optional. The number of clicks the link is valid for.
        /// If expiry_clicks is specified, expiry_date cannot be set.
        /// Value must be between 1 - 10, inclusive.
        /// </summary>
        public int? ExpiryClicks { get; set; }

        /// <summary>
        /// Optional. If True then each recipient's uploaded data will be put into a separate folder.
        /// Only applies to upload links.
        /// </summary>
        public bool? FolderPerRecipient { get; set; }
    }
}
