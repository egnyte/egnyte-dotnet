using System.Collections.Generic;

namespace Egnyte.Api.Permissions
{
    public class FolderPermissions
    {
        internal FolderPermissions(
            List<GroupOrUserPermissions> users,
            List<GroupOrUserPermissions> groups)
        {
            Users = users;
            Groups = groups;
        }

        public List<GroupOrUserPermissions> Users { get; private set; }
        
        public List<GroupOrUserPermissions> Groups { get; private set; }
    }
}
