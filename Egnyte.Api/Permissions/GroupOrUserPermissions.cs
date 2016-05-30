namespace Egnyte.Api.Permissions
{
    public class GroupOrUserPermissions
    {
        internal GroupOrUserPermissions(string subject, PermissionType permission)
        {
            Subject = subject;
            Permission = permission;
        }

        public string Subject { get; private set; }
        
        public PermissionType Permission { get; private set; }
    }
}
