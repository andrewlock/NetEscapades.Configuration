using System.IO;
using Microsoft.Extensions.Configuration;

namespace NetEscapades.Configuration.Yaml
{
    /// <summary>
    /// A YAML stream source implements <see cref="IConfigurationSource"/>.
    /// </summary>
    public class YamlStreamConfigurationSource : IConfigurationSource
    {
        public Stream Stream { get; set; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new YamlStreamConfigurationProvider(this);
        }
    }
}