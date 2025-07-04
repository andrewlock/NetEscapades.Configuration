using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using YamlDotNet.Core;

namespace NetEscapades.Configuration.Yaml
{
    /// <summary>
    /// A YAML file based <see cref="FileConfigurationProvider"/>.
    /// </summary>
    public class YamlConfigurationProvider : FileConfigurationProvider
    {
        private readonly YamlConfigurationSource _source;

        public YamlConfigurationProvider(YamlConfigurationSource source) : base(source)
        {
            _source = source;
        }

        public override void Load(Stream stream)
        {
            var parser = new YamlConfigurationStreamParser(_source.ConfigureDeserializer);
            try
            {
                Data = parser.Parse(stream);
            }
            catch (YamlException e)
            {
                throw new FormatException(Resources.FormatError_YamlParseError(e.Message), e);
            }
        }
    }
}
