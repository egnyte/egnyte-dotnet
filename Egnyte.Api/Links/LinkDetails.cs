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

        public string Path { get; private set; }
        
        public LinkType Type { get; private set; }
        
        public LinkAccessibility Accessibility { get; private set; }
        
        public bool Notify { get; private set; }
        
        public bool LinkToCurrent { get; private set; }
        
        public DateTime CreationDate { get; private set; }
        
        public string CreatedBy { get; private set; }
        
        public ProtectionType Protection { get; private set; }
        
        public List<string> Recipients { get; private set; }
        
        public string Url { get; private set; }
        
        public string Id { get; private set; }
    }
}
