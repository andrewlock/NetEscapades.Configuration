using System;
using Microsoft.Extensions.Configuration;
using YamlDotNet.Serialization;

namespace NetEscapades.Configuration.Yaml
{
    /// <summary>
    /// A YAML file based <see cref="FileConfigurationSource"/>.
    /// </summary>
    public class YamlConfigurationSource : FileConfigurationSource
    {
        public Action<DeserializerBuilder> ConfigureDeserializer { get; set; }

        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new YamlConfigurationProvider(this);
        }
    }
}
