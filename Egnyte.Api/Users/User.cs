namespace Egnyte.Api.Users
{
    public class User
    {
        /// <summary>
        /// Required. The Egnyte username for the user. Username must start with a letter or digit.
        /// Special characters are not supported (with the exception of periods, hyphens, and underscores).
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Required. This is an immutable unique identifier provided by the API consumer.
        /// Any plain text (e.g. S-1-5-21-3623811015-3361044348-30300820-1013).
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Required. The email address of the user. Any valid email address (e.g. admin@acme.com).
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Required. The last name of the user. Any plain text (e.g. Smith).
        /// </summary>
        public string FamilyName { get; set; }

        /// <summary>
        /// Required. The first name of the user. Any plain text (e.g. John).
        /// </summary>
        public string GivenName { get; set; }

        /// <summary>
        /// Required. Whether the user is active or inactive.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Required. The authentication type for the user.
        /// </summary>
        public UserAuthType AuthType { get; set; }

        /// <summary>
        /// Required. The type of the user.
        /// </summary>
        public UserType UserType { get; set; }

        /// <summary>
        /// Optional. The role assigned to the user. Only applicable for Power Users.
        /// Value can be 'Default' or custom role name
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Optional. Only required if the user is SSO authenticated and not using default user mapping.
        /// Do not specify if user is not SSO authenticated. This is the way the user is identified within
        /// the SAML Response from an SSO Identity Provider, i.e. the SAML Subject (e.g. jsmith)
        /// </summary>
        public string IdpUserId { get; set; }

        /// <summary>
        /// Optional. Do not specify if user is not AD authenticated. Used to bind child authentication policies
        /// to a user when using Active Directory authentication in a multi-domain setup (e.g. jmiller@example.com)
        /// </summary>
        public string UserPrincipalName { get; set; }
    }
}
