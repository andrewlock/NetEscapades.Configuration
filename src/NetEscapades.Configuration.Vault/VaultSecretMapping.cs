namespace NetEscapades.Configuration.Vault
{
    public readonly struct VaultSecretMapping
    {
        /// <summary>
        /// Optional prefix, defining path in IConfiguration. Null for root.
        /// </summary>
        public string Prefix { get; }

        /// <summary>
        /// Secret relative url.
        /// </summary>
        public string VaultPath { get; }
  
        public VaultSecretMapping(string prefix, string vaultPath)
        {
            Prefix = prefix;
            VaultPath = vaultPath;
        }
    }
}