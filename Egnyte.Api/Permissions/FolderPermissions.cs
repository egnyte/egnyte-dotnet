using System.Collections.Generic;

namespace Egnyte.Api.Permissions
{
    public class FolderPermissions
    {
        public List<GroupOrUserPermissions> Users { get; set; }
        
        public List<GroupOrUserPermissions> Groups { get; set; }
    }
}
