namespace Egnyte.Api.Groups
{
    public class GroupMember
    {
        /// <summary>
        /// Creates new member of a group
        /// </summary>
        /// <param name="id">The globally unique id of a group member</param>
        /// <param name="deleteUser">Specifies if user should be deleted</param>
        public GroupMember(long id, bool? deleteUser = null)
        {
            Id = id;
            
            if (deleteUser.HasValue)
            {
                DeleteUser = deleteUser.Value;
            }
        }

        /// <summary>
        /// The globally unique id of a group member
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Specifies if user should be deleted
        /// </summary>
        public bool DeleteUser { get; private set; }
    }
}
