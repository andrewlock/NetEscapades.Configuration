using VaultSharp.V1.Core;

namespace VaultSharp.V1.AuthMethods.Token
{
    /// <summary>
    /// Represents the login information for the Token Authentication backend.
    /// </summary>
    public class AppRoleAuthInfo : IAuthInfo
    {
        /// <summary>
        /// Gets the type of the authentication backend.
        /// </summary>
        /// <value>
        /// The type of the authentication backend.
        /// </value>
        public AuthMethodType BackendType => AuthMethodType.AppRole;

        /// <summary>
        /// Gets the roleid.
        /// </summary>
        /// <value>
        /// The roleId.
        /// </value>
        public string RoleId { get; }

        /// <summary>
        /// Gets the secretId.
        /// </summary>
        /// <value>
        /// The secretId.
        /// </value>
        public string SecretId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppRoleAuthInfo" /> class.
        /// </summary>
        /// <param name="vaultToken">The token.</param>
        public AppRoleAuthInfo(string roleId, string secretId)
        {
            Checker.NotNull(roleId, "roleId");
            Checker.NotNull(secretId, "secretId");

            this.RoleId = roleId;
            this.SecretId = secretId;
        }
    }
}