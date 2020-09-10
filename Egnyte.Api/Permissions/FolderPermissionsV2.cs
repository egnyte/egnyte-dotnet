using System.Collections.Generic;

namespace Egnyte.Api.Permissions
{
    public class FolderPermissionsV2 : FolderPermissions
    {
        internal FolderPermissionsV2(
            List<GroupOrUserPermissions> users,
            List<GroupOrUserPermissions> groups,
            bool inheritsPermissions) : base(users, groups)
        {
            InheritsPermissions = inheritsPermissions;
        }

        public bool InheritsPermissions { get; private set; }
    }
}
