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
            string password,
            bool notify,
            bool linkToCurrent,
            DateTime expiryDate,
            DateTime creationDate,
            string createdBy,
            bool sendMail,
            bool copyMe)
        {
            Links = links;
            Path = path;
            Type = type;
            Accessibility = accessibility;
            Password = password;
            Notify = notify;
            LinkToCurrent = linkToCurrent;
            ExpiryDate = expiryDate;
            CreationDate = creationDate;
            CreatedBy = createdBy;
            SendMail = sendMail;
            CopyMe = copyMe;
        }

        public List<Link> Links { get; private set; }
        
        public string Path { get; private set; }
        
        public LinkType Type { get; private set; }
        
        public LinkAccessibility Accessibility { get; private set; }

        public string Password { get; private set; }
        
        public bool Notify { get; private set; }
        
        public bool LinkToCurrent { get; private set; }
        
        public DateTime ExpiryDate { get; private set; }
        
        public DateTime CreationDate { get; private set; }
        
        public string CreatedBy { get; private set; }
        
        public bool SendMail { get; private set; }
        
        public bool CopyMe { get; private set; }
    }
}
