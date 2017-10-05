// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Extensions.FileProviders;
using NetEscapades.Configuration.KubeSecrets;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Extension methods for adding <see cref="KubeSecretsConfigurationProvider"/>.
    /// </summary>
    public static class KubeSecretsConfigurationExtensions
    {
        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Kubernetes secrets.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="secretsPath">The path to the secrets directory.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddKubeSecrets(this IConfigurationBuilder builder, string secretsPath)
            => builder.AddKubeSecrets(secretsPath, optional: false);

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Kubernetes secrets.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="secretsPath">The path to the secrets directory.</param>
        /// <param name="optional">Whether the directory is optional.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddKubeSecrets(this IConfigurationBuilder builder, string secretsPath, bool optional)
        {
            if (builder == null) { throw new ArgumentNullException(nameof(builder)); }
            if (string.IsNullOrEmpty(secretsPath)) { throw new ArgumentException("File path must be a non-empty string.", nameof(secretsPath)); }

            var source = new KubeSecretsConfigurationSource
            {
                SecretsDirectory = secretsPath,
                Optional = optional
            };

            return builder.Add(source);
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Kubernetes secrets.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="provider">The <see cref="IFileProvider"/> to use to access the files.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddKubeSecrets(this IConfigurationBuilder builder, IFileProvider provider)
            => builder.AddKubeSecrets(provider, optional: false);


        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Kubernetes secrets.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="provider">The <see cref="IFileProvider"/> to use to access the files.</param>
        /// <param name="optional">Whether the directory is optional.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddKubeSecrets(this IConfigurationBuilder builder, IFileProvider provider, bool optional)
        {
            if (builder == null) { throw new ArgumentNullException(nameof(builder)); }
            if (provider == null) { throw new ArgumentNullException(nameof(provider)); }
            
            var source = new KubeSecretsConfigurationSource
            {
                FileProvider = provider,
                Optional = optional
            };

            return builder.Add(source);
        }
    }
}