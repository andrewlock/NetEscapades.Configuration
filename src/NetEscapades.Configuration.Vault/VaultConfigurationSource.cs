using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using VaultSharp;

namespace NetEscapades.Configuration.Vault
{
    /// <summary>
    /// Represents Hashicorp Vault secrets as an <see cref="IConfigurationSource"/>.
    /// </summary>
    internal class VaultConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// Gets or sets the <see cref="IVaultClient"/> to use for retrieving values.
        /// </summary>
        public IVaultClient Client { get; set; }

        /// <summary>
        /// Gets or sets the secrets path (including storage location) for all of the secrets to load
        /// </summary>
        public IEnumerable<string> SecretLocationPaths { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IVaultSecretManager"/> instance used to control secret loading.
        /// </summary>
        public IVaultSecretManager Manager { get; set; }
        
        /// <inheritdoc />
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new VaultConfigurationProvider(Client, Manager, SecretLocationPaths);
        }
    }
}
