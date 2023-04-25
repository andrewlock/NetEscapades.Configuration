// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using NetEscapades.Configuration.Tests.Common;
using NetEscapades.Configuration.Yaml;
using Xunit;

namespace NetEscapades.Configuration.Yaml
{
    public class YamlConfigurationExtensionsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void AddYamlFile_ThrowsIfFilePathIsNullOrEmpty(string path)
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder();

            // Act and Assert
            var ex = Assert.Throws<ArgumentException>(() => YamlConfigurationExtensions.AddYamlFile(configurationBuilder, path));
            Assert.Equal("path", ex.ParamName);
            Assert.StartsWith("File path must be a non-empty string.", ex.Message);
        }

        [Fact]
        public void AddYamlFile_ThrowsIfFileDoesNotExistAtPathAndIsOptional()
        {
            // Arrange
            var path = "file-does-not-exist.Yaml";

            // Act and Assert
            var ex = Assert.Throws<FileNotFoundException>(() => new ConfigurationBuilder().AddYamlFile(path).Build());
            Assert.StartsWith($"The configuration file '{path}' was not found and is not optional.", ex.Message);
        }
        
        [Fact]
        public void AddYamlFile_DoesNotThrowIfFileDoesNotExistAndIsOptional()
        {
            // Arrange
            var path = "file-does-not-exist.Yaml";

            // Act and Assert
            new ConfigurationBuilder().AddYamlFile(path, optional: true).Build();
        }
        
        [Fact]
        public void AddYamlFile_DoesNotThrowIfAbsolutePathDirectoryDoesNotExist()
        {
            // Arrange
            var path = Path.Combine(Directory.GetCurrentDirectory(), "does", "not", "exist", "file-does-not-exist.Yaml");
            
            // Act and Assert
            new ConfigurationBuilder()
                .AddYamlFile(path, optional: true)
                .Build();
        }
        
        [Fact]
        public void AddYamlStream_ThrowIfIsNull()
        {
            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => new ConfigurationBuilder().AddYamlStream(null).Build());
        }
        
        [Fact]
        public void AddYamlStream_DoesNotThrowIfIsNotNull()
        {
            // Arrange
            var stream = TestStreamHelpers.StringToStream("Test: test");
            
            // Act and Assert
            new ConfigurationBuilder().AddYamlStream(stream).Build();
        }
    }
}
