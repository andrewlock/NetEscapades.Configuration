namespace NetEscapades.Configuration.Yaml
{
    /// <summary>
    /// A YAML stream based <see cref="StreamConfigurationProvider"/>.
    /// </summary>
    public class YamlStreamConfigurationProvider : StreamConfigurationProvider
    {
        public YamlStreamConfigurationProvider(YamlStreamConfigurationSource source) : base(source) { }

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