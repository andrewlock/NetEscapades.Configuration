using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VaultSharp;
using VaultSharp.Backends.System.Models;

namespace NetEscapades.Configuration.Vault
{
    /// <summary>
    /// A Vault based <see cref="ConfigurationProvider"/>.
    /// </summary>
    internal class VaultConfigurationProvider : ConfigurationProvider
    {
        const string DataKey = "data";
        const string MetadataKey = "metadata";
        private readonly IVaultClient _client;
        private IEnumerable<string> _secretPaths;
        private readonly IVaultSecretManager _manager;

        /// <summary>
        /// Creates a new instance of <see cref="VaultConfigurationProvider"/>.
        /// </summary>
        /// <param name="client">The <see cref="IVaultClient"/> to use for retrieving values.</param>
        /// <param name="secretPath">The secret path to read</param>
        /// <param name="manager"></param>
        public VaultConfigurationProvider(IVaultClient client, IVaultSecretManager manager, IEnumerable<string> secretPaths)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _secretPaths = secretPaths ?? throw new ArgumentNullException(nameof(secretPaths));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        public override void Load() => LoadAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        private async Task LoadAsync()
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var secretPath in _secretPaths)
            {
                var secret = await _client.ReadSecretAsync(secretPath).ConfigureAwait(false);

                var secretData = secret.Data;

                // try and guess if we're using v2 KV secrets
                if (secretData.ContainsKey(MetadataKey)
                    && secretData.TryGetValue(DataKey, out var val)
                    && val is IDictionary<string, JToken> v2Data)
                {
                    AddSecrets(data, secret, v2Data);
                }
                else
                {
                    AddSecrets(data, secret, secretData);
                }
            }

            Data = data;
        }

        private void AddSecrets<T>(Dictionary<string, string> data, Secret<Dictionary<string, object>> secret, IDictionary<string, T> secretData)
        {
            foreach (var kvp in secretData)
            {
                if (!_manager.Load(secret, kvp.Key))
                {
                    continue;
                }

                var key = _manager.GetKey(secret, kvp.Key);
                data.Add(key, kvp.Value?.ToString());
            }
        }

    }
}
