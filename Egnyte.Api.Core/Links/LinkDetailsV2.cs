using System;
using System.Collections.Generic;

namespace Egnyte.Api.Links
{
    public class LinkDetailsV2 : LinkDetails
    {
        internal LinkDetailsV2(
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
            string id,
            string resourceId,
            int expiry_clicks,
            DateTime expiry_date)
            : base(
                  path,
                  type,
                  accessibility,
                  notify,
                  linkToCurrent,
                  creationDate,
                  createdBy,
                  protection,
                  recipients,
                  url,
                  id)
        {
            ResourceId = resourceId;
            ExpiryClicks = expiry_clicks;
            ExpiryDate = expiry_date;
        }

        /// <summary>
        /// Id of the associated resource, either group id if it is a file link,
        /// or the folder id if it is a folder or upload link
        /// </summary>
        public string ResourceId { get; private set; }

        /// <summary>
        /// Number of clicks left on the link before expiration.
        /// This field is only shown if the link is to expire via clicks
        /// </summary>
        public int ExpiryClicks { get; private set; }

        /// <summary>
        /// Date and time of the expiration of the link.
        /// This field is only shown if the link is to expire by date
        /// </summary>
        public DateTime ExpiryDate { get; private set; }
    }
}
