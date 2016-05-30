using System;
using System.Collections.Generic;

namespace Egnyte.Api.Links
{
    public class CreatedLink
    {
        internal CreatedLink(
            List<Link> links,
            string path,
            LinkType type,
            LinkAccessibility accessibility,
            bool notify,
            bool linkToCurrent,
            DateTime expiryDate,
            DateTime creationDate,
            string createdBy)
        {
            Links = links;
            Path = path;
            Type = type;
            Accessibility = accessibility;
            Notify = notify;
            LinkToCurrent = linkToCurrent;
            ExpiryDate = expiryDate;
            CreationDate = creationDate;
            CreatedBy = createdBy;
        }

        public List<Link> Links { get; private set; }
        
        public string Path { get; private set; }
        
        public LinkType Type { get; private set; }
        
        public LinkAccessibility Accessibility { get; private set; }
        
        public bool Notify { get; private set; }
        
        public bool LinkToCurrent { get; private set; }
        
        public DateTime ExpiryDate { get; private set; }
        
        public DateTime CreationDate { get; private set; }
        
        public string CreatedBy { get; private set; }
    }
}
