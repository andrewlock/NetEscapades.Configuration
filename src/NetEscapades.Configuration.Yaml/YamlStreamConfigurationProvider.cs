using System;
using Microsoft.Extensions.Configuration;
using YamlDotNet.Core;

namespace NetEscapades.Configuration.Yaml
{
    /// <summary>
    /// A YAML stream provider based <see cref="ConfigurationProvider"/>.
    /// </summary>
    public class YamlStreamConfigurationProvider : ConfigurationProvider
    {
        private readonly YamlStreamConfigurationSource _source;

        public YamlStreamConfigurationProvider(YamlStreamConfigurationSource source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }


        public override void Load()
        {
            var parser = new YamlConfigurationStreamParser();

            try
            {
                Data = parser.Parse(_source.Stream);
            }
            catch (YamlException e)
            {
                throw new FormatException(Resources.FormatError_YamlParseError(e.Message), e);
            }
        }
    }
}