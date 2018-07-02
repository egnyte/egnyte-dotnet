using System.Collections.Generic;

namespace Egnyte.Api.Links
{
    public class LinksListV2
    {
        public LinksListV2(List<LinkDetailsV2> links, int count)
        {
            Links = links;
            Count = count;
        }

        /// <summary>
        /// An array containing the full json information for all links for your domain
        /// that this user can see within any specified ranges
        /// </summary>
        public List<LinkDetailsV2> Links { get; private set; }

        /// <summary>
        /// The number of links visible to the user in the domain that are returned
        /// </summary>
        public int Count { get; private set; }
    }
}
