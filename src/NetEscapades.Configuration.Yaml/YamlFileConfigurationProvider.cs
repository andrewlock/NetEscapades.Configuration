using Microsoft.Extensions.Configuration;

namespace NetEscapades.Configuration.Yaml
{
    /// <summary>
    /// A YAML file based <see cref="YamlFileConfigurationProvider"/>.
    /// </summary>
    public class YamlFileConfigurationProvider : FileConfigurationProvider
    {
        public YamlFileConfigurationProvider(YamlFileConfigurationSource source) : base(source) { }

        public override void Load(Stream stream)
        {
            var parser = new YamlConfigurationStreamParser();
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
