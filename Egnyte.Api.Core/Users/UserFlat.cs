namespace Egnyte.Api.Users
{
    public class UserFlat
    {
        public string userName { get; set; }

        public string externalId { get; set; }

        public string email { get; set; }

        public UserName name { get; set; }

        public bool active { get; set; }

        public bool locked { get; set; }

        public string authType { get; set; }

        public string userType { get; set; }

        public string idpUserId { get; set; }

        public string role { get; set; }

        public string userPrincipalName { get; set; }
    }
}
