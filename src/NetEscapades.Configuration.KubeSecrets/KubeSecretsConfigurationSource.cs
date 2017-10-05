using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace NetEscapades.Configuration.KubeSecrets
{
    /// <summary>
    /// An <see cref="IConfigurationSource"/> for Kubernetes secrets.
    /// </summary>
    public class KubeSecretsConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// The secrets directory which will be used if FileProvider is not set.
        /// </summary>
        public string SecretsDirectory { get; set; }

        /// <summary>
        /// The FileProvider representing the secrets directory.
        /// </summary>
        public IFileProvider FileProvider { get; set; }

        /// <summary>
        /// Used to determine if a file should be ignored using its name.
        /// </summary>
        public Func<string, bool> IgnoreCondition { get; set; }

        /// <summary>
        /// If false, will throw if the secrets directory doesn't exist.
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        /// Builds the <see cref="DockerSecretsConfigurationProvider"/> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>A <see cref="DockerSecretsConfigurationProvider"/></returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new KubeSecretsConfigurationProvider(this);
        }
    }
}
