using System.Collections.Generic;

namespace Egnyte.Api.Users
{
    public class UserList
    {
        /// <summary>
        /// Non-negative Integer. Specifies the total number
        /// of results matching the query; e.g., 1000.
        /// </summary>
        public int TotalResults { get; set; }

        /// <summary>
        /// Non-negative Integer. Specifies the number of search results
        /// returned in a query response page; e.g., 50.
        /// </summary>
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// The 1-based index of the first result in the current
        /// set of search results; e.g., 1.
        /// </summary>
        public int StartIndex { get; set; }
        
        /// <summary>
        /// Matching users
        /// </summary>
        public List<ExistingUser> Users { get; set; }
    }
}
