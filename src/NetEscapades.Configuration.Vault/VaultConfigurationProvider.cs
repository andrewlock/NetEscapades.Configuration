﻿using Microsoft.Extensions.Configuration;
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
        private readonly IEnumerable<VaultSecretMapping> _secretPaths;
        readonly bool _asJson;
        private readonly IVaultSecretManager _manager;

        /// <summary>
        /// Creates a new instance of <see cref="VaultConfigurationProvider"/>.
        /// </summary>
        /// <param name="client">The <see cref="IVaultClient"/> to use for retrieving values.</param>
        /// <param name="secretPaths">The secret paths to read</param>
        /// <param name="manager"></param>
        /// <param name="asJson">Read as JSON</param>
        public VaultConfigurationProvider(IVaultClient client, IVaultSecretManager manager, IEnumerable<VaultSecretMapping> secretPaths, bool asJson)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _secretPaths = secretPaths ?? throw new ArgumentNullException(nameof(secretPaths));
            _asJson = asJson;
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        public override void Load() => LoadAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        private async Task LoadAsync()
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var secretPath in _secretPaths)
            {
                var secret = await _client.ReadSecretAsync(secretPath.VaultPath).ConfigureAwait(false);

                var secretData = secret.Data;

                // try and guess if we're using v2 KV secrets
                if (secretData.ContainsKey(MetadataKey)
                    && secretData.TryGetValue(DataKey, out var val)
                    && val is IDictionary<string, JToken> v2Data)
                {
                    AddSecrets(data, secretPath.Prefix, secret, v2Data);
                }
                else
                {
                    AddSecrets(data, secretPath.Prefix, secret, secretData);
                }
            }

            Data = data;
        }

        private void AddSecrets<T>(Dictionary<string, string> data, string pathPrefix,
            Secret<Dictionary<string, object>> secret, IDictionary<string, T> secretData)
        {
            foreach (var kvp in secretData)
            {
                if (!_manager.Load(secret, kvp.Key))
                {
                    continue;
                }
                
                if (_asJson)
                {
                    var configInner = JsonConfigurationStringParser.Parse(kvp.Value?.ToString());
                    foreach (var inner in configInner)
                    {
                        var key = string.IsNullOrWhiteSpace(pathPrefix)
                            ? inner.Key
                            : $"{pathPrefix}:{inner.Key}";
                        
                        data.Add(key, inner.Value);
                    }
                }
                else
                {
                    var key = string.IsNullOrWhiteSpace(pathPrefix)
                        ? _manager.GetKey(secret, kvp.Key)
                        : $"{pathPrefix}:{_manager.GetKey(secret, kvp.Key)}";
                        
                    data.Add(key, kvp.Value?.ToString());
                }
            }
        }
    }
}
