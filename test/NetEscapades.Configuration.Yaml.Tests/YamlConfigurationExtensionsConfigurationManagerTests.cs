using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using NetEscapades.Configuration.Tests.Common;
using Xunit;

#if NET6_0_OR_GREATER

namespace NetEscapades.Configuration.Yaml
{
    public class YamlConfigurationExtensionsConfigurationManagerTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void AddYamlFile_ThrowsIfFilePathIsNullOrEmpty(string path)
        {
            // Arrange
            var configurationManager = new ConfigurationManager();

            // Act and Assert
            var ex = Assert.Throws<ArgumentException>(() => YamlConfigurationExtensions.AddYamlFile(configurationManager, path));
            Assert.Equal("path", ex.ParamName);
            Assert.StartsWith("File path must be a non-empty string.", ex.Message);
        }

        [Fact]
        public void AddYamlFile_ThrowsIfFileDoesNotExistAtPathAndIsOptional()
        {
            // Arrange
            var path = "file-does-not-exist.Yaml";

            // Act and Assert
            var ex = Assert.Throws<FileNotFoundException>(() => new ConfigurationManager().AddYamlFile(path).Build());
            Assert.StartsWith($"The configuration file '{path}' was not found and is not optional.", ex.Message);
        }
        
        [Fact]
        public void AddYamlFile_DoesNotThrowIfFileDoesNotExistAndIsOptional()
        {
            // Arrange
            var path = "file-does-not-exist.Yaml";

            // Act and Assert
            new ConfigurationManager().AddYamlFile(path, optional: true).Build();
        }
        
        [Fact]
        public void AddYamlFile_DoesNotThrowIfAbsolutePathDirectoryDoesNotExist()
        {
            // Arrange
            var path = Path.Combine(Directory.GetCurrentDirectory(), "does", "not", "exist", "file-does-not-exist.Yaml");
            
            // Act and Assert
            new ConfigurationManager()
                .AddYamlFile(path, optional: true)
                .Build();
        }
        
        [Fact]
        public void AddYamlStream_ThrowIfIsNull()
        {
            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => new ConfigurationManager().AddYamlStream(null).Build());
        }
        
        [Fact]
        public void AddYamlStream_DoesNotThrowIfIsNotNull()
        {
            // Arrange
            var stream = TestStreamHelpers.StringToStream("Test: test");
            
            // Act and Assert
            var builder = new ConfigurationManager().AddYamlStream(stream);
            var config = builder.Build();
            Assert.Equal("test", config["Test"]);
        }

        [Fact]
        public void AddYamlStream_SupportsLoadingDataTwice()
        {
            // Arrange
            var stream = TestStreamHelpers.StringToStream("Test: test");
            
            var builder = new ConfigurationManager()
                .AddYamlStream(stream);
            builder.Build();

            var config = builder.Build();
            Assert.Equal("test", config["Test"]);
        }

        [Fact]
        public void AddYamlStream_SupportsDisposingStream()
        {
            // Arrange
            var builder = new ConfigurationManager();
            using (var stream = TestStreamHelpers.StringToStream("Test: test"))
            {
                builder.AddYamlStream(stream);
            }
            
            Assert.Equal("test", builder["Test"]);
        }
    }
}

#endif