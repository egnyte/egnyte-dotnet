using Egnyte.Api.Users;

namespace Egnyte.Api.Tasks
{
    public class TaskUser
    {
        public TaskUser(
            long id,
            string username,
            string firstName,
            string lastName,
            string email,
            bool active,
            UserType type
            )
        {
            this.Id = id;
            this.Username = username;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.Active = active;
            this.Type = type;
        }

        public long Id { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool Active { get; set; }

        public UserType Type { get; set; }
    }
}