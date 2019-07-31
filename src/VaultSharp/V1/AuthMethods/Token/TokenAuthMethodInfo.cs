namespace VaultSharp.V1.AuthMethods.Token
{
    /// <summary>
    /// Represents the login information for the Token Authentication backend.
    /// </summary>
    public class TokenAuthMethodInfo : AbstractAuthMethodInfo
    {
        /// <summary>
        /// Gets the type of the authentication backend.
        /// </summary>
        /// <value>
        /// The type of the authentication backend.
        /// </value>
        public override AuthMethodType AuthMethodType => AuthMethodType.Token;

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public string VaultToken { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenAuthMethodInfo" /> class.
        /// </summary>
        /// <param name="vaultToken">The token.</param>
        public TokenAuthMethodInfo(string vaultToken)
        {
            Checker.NotNull(vaultToken, "vaultToken");

            this.VaultToken = vaultToken;
        }
    }
}