using Microsoft.Extensions.Configuration;

namespace NetEscapades.Configuration.Yaml
{
    /// <summary>
    /// A YAML file based <see cref="YamlFileConfigurationSource"/>.
    /// </summary>
    public class YamlFileConfigurationSource : FileConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new YamlFileConfigurationProvider(this);
        }
    }
}