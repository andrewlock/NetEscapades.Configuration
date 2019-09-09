using NetEscapades.Configuration.Vault;
using System;
using VaultSharp;
using VaultSharp.Backends.Authentication.Models;
using VaultSharp.Backends.Authentication.Models.AppRole;
using VaultSharp.Backends.Authentication.Models.Token;
using VaultSharp.Backends.Authentication.Models.UsernamePassword;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Extension methods for registering <see cref="VaultConfigurationProvider"/> with <see cref="IConfigurationBuilder"/>.
    /// </summary>
    public static class VaultConfigurationExtensions
    {
        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Hashicorp Vault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="vaultUri">The Vault uri with port.</param>
        /// <param name="username">The username to use for authentication.</param>
        /// <param name="password">The password to use for authentication.</param>
        /// <param name="secretLocationPaths">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVaultWithUserPass(
            this IConfigurationBuilder configurationBuilder,
            string vaultUri,
            string username,
            string password,
            params string[] secretLocationPaths)
        {
            if (string.IsNullOrWhiteSpace(vaultUri)) { throw new ArgumentException("vaultUri must be a valid URI", nameof(vaultUri)); }
            if (string.IsNullOrEmpty(username)) { throw new ArgumentException("username must not be null or empty", nameof(username)); }
            if (string.IsNullOrEmpty(password)) { throw new ArgumentException("password must not be null or empty", nameof(password)); }

            var authInfo = new UsernamePasswordAuthenticationInfo(username, password);
            return AddVault(configurationBuilder, vaultUri, authInfo, false, secretLocationPaths);
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Hashicorp Vault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="vaultUri">The Vault uri with port.</param>
        /// <param name="roleId">The AppRole role_id to use for authentication.</param>
        /// <param name="secretId">The secret_id to use for authentication.</param>
        /// <param name="secretLocationPaths">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVaultWithAppRole(
            this IConfigurationBuilder configurationBuilder,
            string vaultUri,
            string roleId,
            string secretId,
            params string[] secretLocationPaths)
        {
            if (string.IsNullOrWhiteSpace(vaultUri)) { throw new ArgumentException("vaultUri must be a valid URI", nameof(vaultUri)); }
            if (string.IsNullOrEmpty(roleId)) { throw new ArgumentException("roleId must not be null or empty", nameof(roleId)); }
            if (string.IsNullOrEmpty(secretId)) { throw new ArgumentException("secretId must not be null or empty", nameof(secretId)); }

            var authInfo = new AppRoleAuthenticationInfo(roleId, secretId);
            return AddVault(configurationBuilder, vaultUri, authInfo, false, secretLocationPaths);
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Hashicorp Vault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="vaultUri">The Vault uri with port.</param>
        /// <param name="token">The token to use for authentication.</param>
        /// <param name="secretLocationPaths">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVaultWithToken(
            this IConfigurationBuilder configurationBuilder,
            string vaultUri,
            string token,
            params string[] secretLocationPaths)
        {
            if (string.IsNullOrWhiteSpace(vaultUri)) { throw new ArgumentException("vaultUri must be a valid URI", nameof(vaultUri)); }
            if (string.IsNullOrEmpty(token)) { throw new ArgumentException("token must not be null or empty", nameof(token)); }

            var authInfo = new TokenAuthenticationInfo(token);
            return AddVault(configurationBuilder, vaultUri, authInfo, false, secretLocationPaths);
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Hashicorp Vault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="vaultUri">The Vault uri with port.</param>
        /// <param name="username">The username to use for authentication.</param>
        /// <param name="password">The password to use for authentication.</param>
        /// <param name="secretLocationPaths">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVaultAsJsonWithUserPass(
            this IConfigurationBuilder configurationBuilder,
            string vaultUri,
            string username,
            string password,
            params string[] secretLocationPaths)
        {
            if (string.IsNullOrWhiteSpace(vaultUri)) { throw new ArgumentException("vaultUri must be a valid URI", nameof(vaultUri)); }
            if (string.IsNullOrEmpty(username)) { throw new ArgumentException("username must not be null or empty", nameof(username)); }
            if (string.IsNullOrEmpty(password)) { throw new ArgumentException("password must not be null or empty", nameof(password)); }

            var authInfo = new UsernamePasswordAuthenticationInfo(username, password);
            return AddVault(configurationBuilder, vaultUri, authInfo, true, secretLocationPaths);
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Hashicorp Vault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="vaultUri">The Vault uri with port.</param>
        /// <param name="roleId">The AppRole role_id to use for authentication.</param>
        /// <param name="secretId">The secret_id to use for authentication.</param>
        /// <param name="secretLocationPaths">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVaultAsJsonWithAppRole(
            this IConfigurationBuilder configurationBuilder,
            string vaultUri,
            string roleId,
            string secretId,
            params string[] secretLocationPaths)
        {
            if (string.IsNullOrWhiteSpace(vaultUri)) { throw new ArgumentException("vaultUri must be a valid URI", nameof(vaultUri)); }
            if (string.IsNullOrEmpty(roleId)) { throw new ArgumentException("roleId must not be null or empty", nameof(roleId)); }
            if (string.IsNullOrEmpty(secretId)) { throw new ArgumentException("secretId must not be null or empty", nameof(secretId)); }

            var authInfo = new AppRoleAuthenticationInfo(roleId, secretId);
            return AddVault(configurationBuilder, vaultUri, authInfo, true, secretLocationPaths);
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Hashicorp Vault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="vaultUri">The Vault uri with port.</param>
        /// <param name="token">The token to use for authentication.</param>
        /// <param name="secretLocationPaths">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVaultAsJsonWithToken(
            this IConfigurationBuilder configurationBuilder,
            string vaultUri,
            string token,
            params string[] secretLocationPaths)
        {
            if (string.IsNullOrWhiteSpace(vaultUri)) { throw new ArgumentException("vaultUri must be a valid URI", nameof(vaultUri)); }
            if (string.IsNullOrEmpty(token)) { throw new ArgumentException("token must not be null or empty", nameof(token)); }

            var authInfo = new TokenAuthenticationInfo(token);
            return AddVault(configurationBuilder, vaultUri, authInfo, true, secretLocationPaths);
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Hashicorp Vault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="vaultUri">The Vault uri with port.</param>
        /// <param name="roleId">The AppRole role_id to use for authentication.</param>
        /// <param name="secretId">The secret_id to use for authentication.</param>
        /// <param name="asJson"></param>
        /// <param name="secretLocationPaths">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVault(
            this IConfigurationBuilder configurationBuilder,
            string vaultUri,
            IAuthenticationInfo authenticationInfo,
            bool asJson = false,
            params string[] secretLocationPaths)
        {
            if (string.IsNullOrWhiteSpace(vaultUri)) { throw new ArgumentException("VaultUri must be a valid URI", nameof(vaultUri)); }
            var uri = new Uri(vaultUri);

            var vaultClient = VaultClientFactory.CreateVaultClient(
                uri,
                authenticationInfo,
                continueAsyncTasksOnCapturedContext: false);

            return AddVault(configurationBuilder, vaultClient, new DefaultVaultSecretManager(), asJson, secretLocationPaths);
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Hashicorp Vault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="client">The <see cref="IVaultClient"/> to use for retrieving values.</param>
        /// <param name="manager">The <see cref="IKeyVaultSecretManager"/> instance used to control secret loading.</param>
        /// <param name="asJson">Secrets stored as Vault JSON secrets</param>
        /// <param name="secretLocationPaths">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVault(
            this IConfigurationBuilder configurationBuilder,
            IVaultClient client,
            IVaultSecretManager manager,
            bool asJson = false,
            params string[] secretLocationPaths)
        {
            if (configurationBuilder == null)
            {
                throw new ArgumentNullException(nameof(configurationBuilder));
            }
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (secretLocationPaths == null)
            {
                throw new ArgumentNullException(nameof(secretLocationPaths));
            }
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            configurationBuilder.Add(new VaultConfigurationSource()
            {
                Client = client,
                SecretLocationPaths = secretLocationPaths,
                Manager = manager,
                UsingJsonSecrets = asJson,
            });

            return configurationBuilder;
        }
    }
}
