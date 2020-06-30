namespace Egnyte.Api.Users
{
    public class NewUser : User
    {
        /// <summary>
        /// Required. If set to true when creating a user,
        /// an invitation email will be sent (if the user is created in active state).
        /// </summary>
        public bool SendInvite { get; set; }
    }
}
