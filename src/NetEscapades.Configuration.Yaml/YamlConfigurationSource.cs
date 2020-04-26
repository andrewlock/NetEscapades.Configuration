using Microsoft.Extensions.Configuration;

namespace NetEscapades.Configuration.Yaml
{
    /// <summary>
    /// A YAML file based <see cref="FileConfigurationSource"/>.
    /// </summary>
    public class YamlConfigurationSource : FileConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new YamlConfigurationProvider(this);
        }
    }
}