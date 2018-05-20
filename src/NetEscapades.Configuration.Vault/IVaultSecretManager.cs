using System.Collections.Generic;
using VaultSharp.Backends.System.Models;

namespace NetEscapades.Configuration.Vault
{
    /// <summary>
    /// The <see cref="IVaultSecretManager"/> instance used to control secret loading.
    /// </summary>
    public interface IVaultSecretManager
    {
        /// <summary>
        /// Checks if the value for <paramref name="key"/> should be used.
        /// </summary>
        /// <param name="secret">The <see cref="Secret"/> instance.</param>
        /// <param name="key">The key in the <see cref="Secret{TData}"/>.</param>
        /// <returns><code>true</code> is secrets value should be loaded, otherwise <code>false</code>.</returns>
        bool Load(Secret<Dictionary<string, object>> secret, string key);

        /// <summary>
        /// Maps secret key to a configuration key.
        /// </summary>
        /// <param name="secret">The <see cref="Secret"/> instance.</param>
        /// <param name="secretKey">The secret key instance.</param>
        /// <returns>Configuration key name to store secret value.</returns>
        string GetKey(Secret<Dictionary<string, object>> secret, string secretKey);
    }
}
