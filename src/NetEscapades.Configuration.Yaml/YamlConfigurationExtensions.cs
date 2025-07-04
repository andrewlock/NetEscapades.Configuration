// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.FileProviders;
using NetEscapades.Configuration.Yaml;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Extension methods for adding <see cref="YamlConfigurationExtensions"/>.
    /// </summary>
    public static class YamlConfigurationExtensions
    {
        /// <summary>
        /// Adds the YAML configuration provider at <paramref name="path"/> to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="path">Path relative to the base path stored in
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <param name="configureDeserializer">The action that configures the YAML deserializer.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, string path, Action<DeserializerBuilder> configureDeserializer = null)
        {
            return AddYamlFile(builder, provider: null, path: path, optional: false, reloadOnChange: false, configureDeserializer: configureDeserializer);
        }

        /// <summary>
        /// Adds the YAML configuration provider at <paramref name="path"/> to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="path">Path relative to the base path stored in
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, string path)
        {
            return AddYamlFile(builder, provider: null, path: path, optional: false, reloadOnChange: false, configureDeserializer: null);
        }

        /// <summary>
        /// Adds the YAML configuration provider at <paramref name="path"/> to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="path">Path relative to the base path stored in
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, string path, bool optional)
        {
            return AddYamlFile(builder, provider: null, path: path, optional: optional, reloadOnChange: false, configureDeserializer: null);
        }

        /// <summary>
        /// Adds the YAML configuration provider at <paramref name="path"/> to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="path">Path relative to the base path stored in
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <param name="reloadOnChange">Whether the configuration should be reloaded if the file changes.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange, Action<DeserializerBuilder> configureDeserializer)
        {
            return AddYamlFile(builder, provider: null, path: path, optional: optional, reloadOnChange: reloadOnChange, configureDeserializer: configureDeserializer);
        }

        /// <summary>
        /// Adds a YAML configuration source to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="provider">The <see cref="IFileProvider"/> to use to access the file.</param>
        /// <param name="path">Path relative to the base path stored in
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <param name="reloadOnChange">Whether the configuration should be reloaded if the file changes.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, IFileProvider provider, string path, bool optional, bool reloadOnChange, Action<DeserializerBuilder> configureDeserializer)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(Resources.FormatError_InvalidFilePath(), nameof(path));
            }

            return builder.AddYamlFile(s =>
            {
                s.FileProvider = provider;
                s.Path = path;
                s.Optional = optional;
                s.ReloadOnChange = reloadOnChange;
                s.ConfigureDeserializer = configureDeserializer;
                s.ResolveFileProvider();
            });
        }

        /// <summary>
        /// Adds a YAML configuration source to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="configureSource">Configures the source.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, Action<YamlConfigurationSource> configureSource)
            => builder.Add(configureSource);

        /// <summary>
        /// Adds a YAML configuration source to <paramref name="builder"/> that reads from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="stream">The <see cref="Stream"/> to read the yaml configuration data from.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddYamlStream(this IConfigurationBuilder builder, Stream stream, Action<DeserializerBuilder> configureDeserializer = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var data = ReadStream(stream, configureDeserializer);

            return builder.Add<StaticConfigurationSource>(s => s.Data = data);

            static IDictionary<string, string> ReadStream(Stream s, Action<DeserializerBuilder> configureDeserializer)
            {
                try
                {
                    return new YamlConfigurationStreamParser(configureDeserializer).Parse(s);
                }
                catch (YamlException e)
                {
                    throw new FormatException(Resources.FormatError_YamlParseError(e.Message), e);
                }
            }
        }
    }
}
