using Egnyte.Api.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Egnyte.Api.Users
{
    public class UsersClient
    {
        readonly HttpClient httpClient;

        readonly string domain;

        const string UsersBasePath = "https://{0}.egnyte.com/pubapi/v2/users";

        internal UsersClient(HttpClient httpClient, string domain)
        {
            this.httpClient = httpClient;
            this.domain = domain;
        }

        /// <summary>
        /// Creates single user
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Created user with Id</returns>
        public async Task<ExistingUser> CreateUser(NewUser user)
        {
            ThrowExceptionsIfNewUserIsInvalid(user);

            var uriBuilder = new UriBuilder(string.Format(UsersBasePath, domain));
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
            {
                Content = new StringContent(MapUserForRequest(user), Encoding.UTF8, "application/json")
            };

            var serviceHandler = new ServiceHandler<ExistingUserFlat>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return MapFlatUserToUser(response.Data);
        }

        /// <summary>
        /// Updates user
        /// </summary>
        /// <param name="user">User with fields you want to update</param>
        /// <returns>Updated user</returns>
        public async Task<ExistingUser> UpdateUser(UserUpdate user)
        {
            if (user.Id < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(user.Id));
            }

            var uriBuilder = new UriBuilder(string.Format(UsersBasePath, domain) + "/" + user.Id);
            var httpRequest = new HttpRequestMessage(new HttpMethod("PATCH"), uriBuilder.Uri)
            {
                Content = new StringContent(MapUserForRequest(user), Encoding.UTF8, "application/json")
            };

            var serviceHandler = new ServiceHandler<ExistingUserFlat>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return MapFlatUserToUser(response.Data);
        }

        ExistingUser MapFlatUserToUser(ExistingUserFlat data)
        {
            return new ExistingUser
            {
                Id = long.Parse(data.id),
                UserName = data.userName,
                ExternalId = data.externalId,
                Email = data.email,
                FamilyName = data.name.familyName,
                GivenName = data.name.givenName,
                Active = data.active,
                Locked = data.locked,
                AuthType = MapAuthType(data.authType),
                UserType = MapUserType(data.userType),
                IdpUserId = data.idpUserId,
                Role = data.role,
                UserPrincipalName = data.userPrincipalName
            };
        }

        void ThrowExceptionsIfNewUserIsInvalid(User user)
        {
            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                throw new ArgumentNullException(nameof(user.UserName));
            }

            if (string.IsNullOrWhiteSpace(user.ExternalId))
            {
                throw new ArgumentNullException(nameof(user.ExternalId));
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new ArgumentNullException(nameof(user.Email));
            }

            if (string.IsNullOrWhiteSpace(user.FamilyName))
            {
                throw new ArgumentNullException(nameof(user.FamilyName));
            }

            if (string.IsNullOrWhiteSpace(user.GivenName))
            {
                throw new ArgumentNullException(nameof(user.GivenName));
            }
        }

        string MapUserForRequest(NewUser user)
        {
            var builder = new StringBuilder();
            builder
                .Append("{")
                .Append("\"userName\" : \"" + user.UserName + "\",")
                .Append("\"externalId\" : \"" + user.ExternalId + "\",")
                .Append("\"email\" : \"" + user.Email + "\",")
                .Append("\"name\" : {")
                .Append("\"familyName\" : \"" + user.FamilyName + "\",")
                .Append("\"givenName\" : \"" + user.GivenName + "\"")
                .Append("},")
                .AppendFormat(@"""active"" : ""{0}"",", user.Active ? "true" : "false")
                .AppendFormat(@"""sendInvite"" : ""{0}"",", user.SendInvite ? "true" : "false")
                .Append("\"authType\" : \"" + MapAuthType(user.AuthType) + "\",")
                .Append("\"userType\" : \"" + MapUserType(user.UserType) + "\"");

            if (!string.IsNullOrWhiteSpace(user.Role))
            {
                builder.Append(",\"role\" : \"" + user.Role + "\"");
            }

            if (!string.IsNullOrWhiteSpace(user.IdpUserId))
            {
                builder.Append(",\"idpUserId\" : \"" + user.IdpUserId + "\"");
            }

            if (!string.IsNullOrWhiteSpace(user.IdpUserId))
            {
                builder.Append(",\"userPrincipalName\" : \"" + user.UserPrincipalName + "\"");
            }

            builder.Append("}");

            return builder.ToString();
        }

        string MapUserForRequest(UserUpdate user)
        {
            var parameters = new List<string>();

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                parameters.Add("\"email\" : \"" + user.Email + "\"");
            }

            if (!string.IsNullOrWhiteSpace(user.FamilyName))
            {
                parameters.Add("\"familyName\" : \"" + user.FamilyName + "\"");
            }

            if (!string.IsNullOrWhiteSpace(user.GivenName))
            {
                parameters.Add("\"givenName\" : \"" + user.GivenName + "\"");
            }

            if (user.Active.HasValue)
            {
                parameters.Add(string.Format(@"""active"" : ""{0}""", user.Active.Value ? "true" : "false"));
            }

            if (user.SendInvite.HasValue)
            {
                parameters.Add(string.Format(@"""sendInvite"" : ""{0}""", user.SendInvite.Value ? "true" : "false"));
            }

            if (user.AuthType.HasValue)
            {
                parameters.Add("\"authType\" : \"" + MapAuthType(user.AuthType.Value) + "\"");
            }

            if (user.UserType.HasValue)
            {
                parameters.Add("\"userType\" : \"" + MapUserType(user.UserType.Value) + "\"");
            }

            if (!string.IsNullOrWhiteSpace(user.IdpUserId))
            {
                parameters.Add("\"idpUserId\" : \"" + user.IdpUserId + "\"");
            }

            if (!string.IsNullOrWhiteSpace(user.UserPrincipalName))
            {
                parameters.Add("\"userPrincipalName\" : \"" + user.UserPrincipalName + "\"");
            }

            var content = "{" + string.Join(",", parameters) + "}";

            return content;
        }

        string MapAuthType(UserAuthType authType)
        {
            switch(authType)
            {
                case UserAuthType.SAML_SSO:
                    return "sso";
                case UserAuthType.Internal_Egnyte:
                    return "egnyte";
                default:
                    return "ad";
            }
        }

        UserAuthType MapAuthType(string authType)
        {
            switch (authType)
            {
                case "sso":
                    return UserAuthType.SAML_SSO;
                case "egnyte":
                    return UserAuthType.Internal_Egnyte;
                default:
                    return UserAuthType.AD;
            }
        }

        string MapUserType(UserType userType)
        {
            switch (userType)
            {
                case UserType.Administrator:
                    return "admin";
                case UserType.PowerUser:
                    return "power";
                default:
                    return "standard";
            }
        }

        UserType MapUserType(string userType)
        {
            switch (userType)
            {
                case "admin":
                    return UserType.Administrator;
                case "power":
                    return UserType.PowerUser;
                default:
                    return UserType.StandardUser;
            }
        }
    }
}
