namespace NetEscapades.Configuration.Yaml
{
    /// <summary>
    /// A YAML stream based <see cref="YamlStreamConfigurationSource"/>.
    /// </summary>
    public class YamlStreamConfigurationSource : StreamConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new YamlStreamConfigurationProvider(this);
        }
    }
}