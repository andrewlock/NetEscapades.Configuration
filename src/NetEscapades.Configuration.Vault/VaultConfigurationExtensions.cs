using NetEscapades.Configuration.Vault;
using System;
using System.Linq;
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
        /// <param name="username">The username to use for authentication.</param>
        /// <param name="password">The password to use for authentication.</param>
        /// <param name="vaultSecretMappings">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVaultWithUserPass(
            this IConfigurationBuilder configurationBuilder,
            string vaultUri,
            string username,
            string password,
            params VaultSecretMapping[] vaultSecretMappings)
        {
            if (string.IsNullOrWhiteSpace(vaultUri)) { throw new ArgumentException("vaultUri must be a valid URI", nameof(vaultUri)); }
            if (string.IsNullOrEmpty(username)) { throw new ArgumentException("username must not be null or empty", nameof(username)); }
            if (string.IsNullOrEmpty(password)) { throw new ArgumentException("password must not be null or empty", nameof(password)); }

            var authInfo = new UsernamePasswordAuthenticationInfo(username, password);
            return AddVault(configurationBuilder, vaultUri, authInfo, false, vaultSecretMappings);
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
        /// <param name="roleId">The AppRole role_id to use for authentication.</param>
        /// <param name="secretId">The secret_id to use for authentication.</param>
        /// <param name="vaultSecretMappings">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVaultWithAppRole(
            this IConfigurationBuilder configurationBuilder,
            string vaultUri,
            string roleId,
            string secretId,
            params VaultSecretMapping[] vaultSecretMappings)
        {
            if (string.IsNullOrWhiteSpace(vaultUri)) { throw new ArgumentException("vaultUri must be a valid URI", nameof(vaultUri)); }
            if (string.IsNullOrEmpty(roleId)) { throw new ArgumentException("roleId must not be null or empty", nameof(roleId)); }
            if (string.IsNullOrEmpty(secretId)) { throw new ArgumentException("secretId must not be null or empty", nameof(secretId)); }

            var authInfo = new AppRoleAuthenticationInfo(roleId, secretId);
            return AddVault(configurationBuilder, vaultUri, authInfo, false, vaultSecretMappings);
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
        /// <param name="token">The token to use for authentication.</param>
        /// <param name="vaultSecretMappings">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVaultWithToken(
            this IConfigurationBuilder configurationBuilder,
            string vaultUri,
            string token,
            params VaultSecretMapping[] vaultSecretMappings)
        {
            if (string.IsNullOrWhiteSpace(vaultUri)) { throw new ArgumentException("vaultUri must be a valid URI", nameof(vaultUri)); }
            if (string.IsNullOrEmpty(token)) { throw new ArgumentException("token must not be null or empty", nameof(token)); }

            var authInfo = new TokenAuthenticationInfo(token);
            return AddVault(configurationBuilder, vaultUri, authInfo, false, vaultSecretMappings);
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
        /// <param name="username">The username to use for authentication.</param>
        /// <param name="password">The password to use for authentication.</param>
        /// <param name="vaultSecretMappings">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVaultAsJsonWithUserPass(
            this IConfigurationBuilder configurationBuilder,
            string vaultUri,
            string username,
            string password,
            params VaultSecretMapping[] vaultSecretMappings)
        {
            if (string.IsNullOrWhiteSpace(vaultUri)) { throw new ArgumentException("vaultUri must be a valid URI", nameof(vaultUri)); }
            if (string.IsNullOrEmpty(username)) { throw new ArgumentException("username must not be null or empty", nameof(username)); }
            if (string.IsNullOrEmpty(password)) { throw new ArgumentException("password must not be null or empty", nameof(password)); }

            var authInfo = new UsernamePasswordAuthenticationInfo(username, password);
            return AddVault(configurationBuilder, vaultUri, authInfo, true, vaultSecretMappings);
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
        /// <param name="roleId">The AppRole role_id to use for authentication.</param>
        /// <param name="secretId">The secret_id to use for authentication.</param>
        /// <param name="vaultSecretMappings">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVaultAsJsonWithAppRole(
            this IConfigurationBuilder configurationBuilder,
            string vaultUri,
            string roleId,
            string secretId,
            params VaultSecretMapping[] vaultSecretMappings)
        {
            if (string.IsNullOrWhiteSpace(vaultUri)) { throw new ArgumentException("vaultUri must be a valid URI", nameof(vaultUri)); }
            if (string.IsNullOrEmpty(roleId)) { throw new ArgumentException("roleId must not be null or empty", nameof(roleId)); }
            if (string.IsNullOrEmpty(secretId)) { throw new ArgumentException("secretId must not be null or empty", nameof(secretId)); }

            var authInfo = new AppRoleAuthenticationInfo(roleId, secretId);
            return AddVault(configurationBuilder, vaultUri, authInfo, true, vaultSecretMappings);
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
        /// <param name="token">The token to use for authentication.</param>
        /// <param name="vaultSecretMappings">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVaultAsJsonWithToken(
            this IConfigurationBuilder configurationBuilder,
            string vaultUri,
            string token,
            params VaultSecretMapping[] vaultSecretMappings)
        {
            if (string.IsNullOrWhiteSpace(vaultUri)) { throw new ArgumentException("vaultUri must be a valid URI", nameof(vaultUri)); }
            if (string.IsNullOrEmpty(token)) { throw new ArgumentException("token must not be null or empty", nameof(token)); }

            var authInfo = new TokenAuthenticationInfo(token);
            return AddVault(configurationBuilder, vaultUri, authInfo, true, vaultSecretMappings);
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Hashicorp Vault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="vaultUri">The Vault uri with port.</param>
        /// <param name="authenticationInfo">The authentication information for Vault</param>
        /// <param name="secretLocationPaths">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVault(
            this IConfigurationBuilder configurationBuilder,
            string vaultUri,
            IAuthenticationInfo authenticationInfo,
            params string[] secretLocationPaths)
        {
            return configurationBuilder.AddVault(vaultUri, authenticationInfo, asJson: false, secretLocationPaths);
        }
        
        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Hashicorp Vault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="vaultUri">The Vault uri with port.</param>
        /// <param name="authenticationInfo">The authentication information for Vault</param>
        /// <param name="vaultSecretMappings">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVault(
            this IConfigurationBuilder configurationBuilder,
            string vaultUri,
            IAuthenticationInfo authenticationInfo,
            params VaultSecretMapping[] vaultSecretMappings)
        {
            return configurationBuilder.AddVault(vaultUri, authenticationInfo, asJson: false, vaultSecretMappings);
        }
        
        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Hashicorp Vault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="vaultUri">The Vault uri with port.</param>
        /// <param name="authenticationInfo">The authentication information for Vault</param>
        /// <param name="asJson">Load the secrets as JSON files</param>
        /// <param name="secretLocationPaths">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVault(
            this IConfigurationBuilder configurationBuilder,
            string vaultUri,
            IAuthenticationInfo authenticationInfo,
            bool asJson,
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
        /// <param name="vaultUri">The Vault uri with port.</param>
        /// <param name="authenticationInfo">The authentication information for Vault</param>
        /// <param name="asJson">Load the secrets as JSON files</param>
        /// <param name="vaultSecretMappings">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVault(
            this IConfigurationBuilder configurationBuilder,
            string vaultUri,
            IAuthenticationInfo authenticationInfo,
            bool asJson,
            params VaultSecretMapping[] vaultSecretMappings)
        {
            if (string.IsNullOrWhiteSpace(vaultUri)) { throw new ArgumentException("VaultUri must be a valid URI", nameof(vaultUri)); }
            var uri = new Uri(vaultUri);

            var vaultClient = VaultClientFactory.CreateVaultClient(
                uri,
                authenticationInfo,
                continueAsyncTasksOnCapturedContext: false);

            return AddVault(configurationBuilder, vaultClient, new DefaultVaultSecretManager(), asJson, vaultSecretMappings);
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Hashicorp Vault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="client">The <see cref="IVaultClient"/> to use for retrieving values.</param>
        /// <param name="manager">The <see cref="IVaultSecretManager"/> instance used to control secret loading.</param>
        /// <param name="secretLocationPaths">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVault(
            this IConfigurationBuilder configurationBuilder,
            IVaultClient client,
            IVaultSecretManager manager,
            params string[] secretLocationPaths)
        {
            return configurationBuilder.AddVault(client, manager, asJson: false, secretLocationPaths);
        }
        
        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Hashicorp Vault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="client">The <see cref="IVaultClient"/> to use for retrieving values.</param>
        /// <param name="manager">The <see cref="IVaultSecretManager"/> instance used to control secret loading.</param>
        /// <param name="vaultSecretMappings">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVault(
            this IConfigurationBuilder configurationBuilder,
            IVaultClient client,
            IVaultSecretManager manager,
            params VaultSecretMapping[] vaultSecretMappings)
        {
            return configurationBuilder.AddVault(client, manager, asJson: false, vaultSecretMappings);
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Hashicorp Vault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="client">The <see cref="IVaultClient"/> to use for retrieving values.</param>
        /// <param name="manager">The <see cref="IVaultSecretManager"/> instance used to control secret loading.</param>
        /// <param name="asJson">Are the secrets stored as Vault JSON secrets</param>
        /// <param name="secretLocationPaths">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVault(
            this IConfigurationBuilder configurationBuilder,
            IVaultClient client,
            IVaultSecretManager manager,
            bool asJson,
            params string[] secretLocationPaths)
        {
            if (secretLocationPaths is null) { throw new ArgumentNullException(nameof(secretLocationPaths)); }
            if (!secretLocationPaths.Any()) { throw new ArgumentException($"{nameof(secretLocationPaths)} cannot be empty", nameof(secretLocationPaths)); }

            var secretPathMappings = secretLocationPaths?
                .Select(s => new VaultSecretMapping(string.Empty, s))
                .ToArray();
            
            return AddVaultInternal(configurationBuilder, client, manager, asJson, secretPathMappings);
        }
        
        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Hashicorp Vault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="client">The <see cref="IVaultClient"/> to use for retrieving values.</param>
        /// <param name="manager">The <see cref="IVaultSecretManager"/> instance used to control secret loading.</param>
        /// <param name="asJson">Are the secrets stored as Vault JSON secrets</param>
        /// <param name="vaultSecretMappings">The vault secret mappings to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVault(
            this IConfigurationBuilder configurationBuilder,
            IVaultClient client,
            IVaultSecretManager manager,
            bool asJson,
            params VaultSecretMapping[] vaultSecretMappings)
        {
            return AddVaultInternal(configurationBuilder, client, manager, asJson, vaultSecretMappings);
        }
        
        private static IConfigurationBuilder AddVaultInternal(
            IConfigurationBuilder configurationBuilder,
            IVaultClient client,
            IVaultSecretManager manager,
            bool asJson,
            VaultSecretMapping[] vaultSecretMappings)
        {
            if (configurationBuilder == null)
            {
                throw new ArgumentNullException(nameof(configurationBuilder));
            }
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            
            if (vaultSecretMappings == null)
            {
                throw new ArgumentNullException(nameof(vaultSecretMappings));
            }
            
            if (!vaultSecretMappings.Any())
            {
                throw new ArgumentException($"{nameof(vaultSecretMappings)} cannot be empty", nameof(vaultSecretMappings));
            }
            
            var duplicatePrefixes = vaultSecretMappings
                .Where(p => !string.IsNullOrWhiteSpace(p.Prefix))
                .GroupBy(x => x.Prefix)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            {
                throw new ArgumentException($"{nameof(vaultSecretMappings)} prefixes must be unique", nameof(vaultSecretMappings));
            }
            
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            configurationBuilder.Add(new VaultConfigurationSource()
            {
                Client = client,
                SecretPathMappings = vaultSecretMappings,
                Manager = manager,
                UsingJsonSecrets = asJson,
            });

            return configurationBuilder;
        }
    }
}
