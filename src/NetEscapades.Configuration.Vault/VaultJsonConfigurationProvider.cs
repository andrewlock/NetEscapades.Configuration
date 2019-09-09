using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetEscapades.Configuration.Vault;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VaultSharp;
using VaultSharp.Backends.System.Models;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// A Vault based <see cref="ConfigurationProvider"/>.
    /// </summary>
    internal class VaultJsonConfigurationProvider : ConfigurationProvider
    {
        const string DataKey = "data";
        const string MetadataKey = "metadata";
        private readonly IVaultClient _client;
        private readonly IEnumerable<string> _secretPaths;
        private readonly IVaultSecretManager _manager;

        /// <summary>
        /// Creates a new instance of <see cref="VaultConfigurationProvider"/>.
        /// </summary>
        /// <param name="client">The <see cref="IVaultClient"/> to use for retrieving values.</param>
        /// <param name="manager"></param>
        /// <param name="secretPaths"></param>
        public VaultJsonConfigurationProvider(IVaultClient client, IVaultSecretManager manager, IEnumerable<string> secretPaths)
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

        private static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void AddSecrets<T>(Dictionary<string, string> data, Secret<Dictionary<string, object>> secret, IDictionary<string, T> secretData)
        {
            foreach (var kvp in secretData)
            {
                if (!_manager.Load(secret, kvp.Key))
                {
                    continue;
                }

                if (IsValidJson(kvp.Value?.ToString()))
                {
                    var configInner = JsonConfigurationStringParser.Parse(kvp.Value?.ToString());
                    foreach (var inner in configInner)
                    {
                        data.Add(inner.Key, inner.Value);
                    }
                }
            }
        }
    }
}