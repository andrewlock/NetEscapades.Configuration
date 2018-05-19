using System.Collections.Generic;
using VaultSharp.Backends.System.Models;

namespace NetEscapades.Configuration.Vault
{
    /// <summary>
    /// Default implementation of <see cref="IVaultSecretManager"/> that loads all secrets
    /// </summary>
    public class DefaultVaultSecretManager : IVaultSecretManager
    {
        /// <inheritdoc />
        public string GetKey(Secret<Dictionary<string, object>> secret, string secretKey)
        {
            return secretKey;
        }

        /// <inheritdoc />
        public virtual bool Load(Secret<Dictionary<string, object>> secret, string key)
        {
            return true;
        }
    }
}
