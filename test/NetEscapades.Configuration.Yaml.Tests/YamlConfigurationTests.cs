// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using NetEscapades.Configuration.Tests.Common;
using Xunit;

namespace NetEscapades.Configuration.Yaml
{
    public class YamlConfigurationTest
    {
        private YamlConfigurationProvider LoadProvider(string Yaml)
        {
            var p = new YamlConfigurationProvider(new YamlConfigurationSource { Optional = true });
            p.Load(TestStreamHelpers.StringToStream(Yaml));
            return p;
        }

        [Fact]
        public void LoadKeyValuePairsFromValidYaml()
        {
            var yaml = @"
                firstname: test
                test.last.name: last.name
                residential.address: 
                  street.name: Something street
                  zipcode: '12345'
                ";
            var yamlConfigSrc = LoadProvider(yaml);

            Assert.Equal("test", yamlConfigSrc.Get("firstname"));
            Assert.Equal("last.name", yamlConfigSrc.Get("test.last.name"));
            Assert.Equal("Something street", yamlConfigSrc.Get("residential.address:STREET.name"));
            Assert.Equal("12345", yamlConfigSrc.Get("residential.address:zipcode"));
        }

        [Fact]
        public void LoadMethodCanHandleBlankValue()
        {
            var yaml = @"
            name: 
        ";
            var yamlConfigSrc = LoadProvider(yaml);
            Assert.Equal(string.Empty, yamlConfigSrc.Get("name"));
        }

        [Fact]
        public void LoadMethodCanHandleEmptyValue()
        {
            var yaml = @"
            name: ''
        ";
            var yamlConfigSrc = LoadProvider(yaml);
            Assert.Equal(string.Empty, yamlConfigSrc.Get("name"));
        }

        [Fact]
        public void LoadMethodCanHandleNullInObject()
        {
            var yaml = @"
                firstname: test
                test.suffix: ~
                test.last.name: ''
                residential.address: 
                  street.name: Something street
                  zipcode: null
                ";
            var yamlConfigSrc = LoadProvider(yaml);

            Assert.Equal("test", yamlConfigSrc.Get("firstname"));
            Assert.Null(yamlConfigSrc.Get("test.suffix"));
            Assert.Equal(string.Empty, yamlConfigSrc.Get("test.last.name"));
            Assert.Equal("Something street", yamlConfigSrc.Get("residential.address:STREET.name"));
            Assert.Null(yamlConfigSrc.Get("residential.address:zipcode"));
        }

        [Fact]
        public void LoadMethodCanHandleNullValue()
        {
            var yaml = @"
                nullValue1: null
                nullValue2: Null
                nullValue3: NULL
                nullValue4: ~
                notNull1: NUll
                notNull2: NuLL
            ";

            var yamlConfigSrc = LoadProvider(yaml);
            Assert.Null(yamlConfigSrc.Get("nullValue1"));
            Assert.Null(yamlConfigSrc.Get("nullValue2"));
            Assert.Null(yamlConfigSrc.Get("nullValue3"));
            Assert.Null(yamlConfigSrc.Get("nullValue4"));
            Assert.NotNull(yamlConfigSrc.Get("notNull1"));
            Assert.NotNull(yamlConfigSrc.Get("notNull2"));
        }

        [Fact]
        public void OverrideWithNullValue()
        {
            var yaml1 = @"
                firstname: test
                ";

            var yaml2 = @"
                firstname: null
                ";

            var yamlConfigSource1 = new YamlConfigurationSource { FileProvider = TestStreamHelpers.StringToFileProvider(yaml1) };
            var yamlConfigSource2 = new YamlConfigurationSource { FileProvider = TestStreamHelpers.StringToFileProvider(yaml2) };

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.Add(yamlConfigSource1);
            configurationBuilder.Add(yamlConfigSource2);
            var config = configurationBuilder.Build();

            Assert.Null(config["firstname"]);
        }

        [Fact]
        public void SupportAndIgnoreComments()
        {
            var yaml = @"# Comments 
                        # Comments
                        name: test
                        # Comments
                        address: 
                          street: Something street # Comments
                          zipcode: '12345'
                    ";
            var yamlConfigSrc = LoadProvider(yaml);
            Assert.Equal("test", yamlConfigSrc.Get("name"));
            Assert.Equal("Something street", yamlConfigSrc.Get("address:street"));
            Assert.Equal("12345", yamlConfigSrc.Get("address:zipcode"));
        }


        [Fact]
        public void ReturnEmptyConfigWhenFileIsEmpty()
        {
            var yaml = @"";

            var yamlConfigSrc = new YamlConfigurationSource { FileProvider = TestStreamHelpers.StringToFileProvider(yaml) };

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.Add(yamlConfigSrc);
            var config = configurationBuilder.Build();

            Assert.Empty(config.AsEnumerable());
        }

        [Fact]
        public void ThrowExceptionWhenUnexpectedFirstCharacterInScalarValue()
        {
            var yaml = @"
                        name: test
                        address: 
                          # Can't start scalar values with unclosed {
                          street: {Something street
                          zipcode: '12345'
                         ";
            var exception = Assert.Throws<FormatException>(() => LoadProvider(yaml));
            Assert.NotNull(exception.Message);
        }

        [Fact]
        public void ThrowExceptionWhenUnexpectedFirstCharacterInKeyValue()
        {
            var yaml = @"
                        name: test
                        address: 
                          street: Something street
                          zipcode: '12345'
                        # Can't start left value with {
                        {phone: mobile 
                         ";
            var exception = Assert.Throws<FormatException>(() => LoadProvider(yaml));
            Assert.NotNull(exception.Message);
        }

        [Fact]
        public void ThrowExceptionWhenUnexpectedEndOfFile()
        {
            var yaml = @"
                        name: test
                        address 
                         ";
            var exception = Assert.Throws<FormatException>(() => LoadProvider(yaml));
            Assert.NotNull(exception.Message);
        }

        [Fact]
        public void ThrowExceptionWhenPassingNullAsFilePath()
        {
            var expectedMsg = new ArgumentException(Resources.Error_InvalidFilePath, "path").Message;

            var exception = Assert.Throws<ArgumentException>(() => new ConfigurationBuilder().AddYamlFile(path: null));

            Assert.Equal(expectedMsg, exception.Message);
        }

        [Fact]
        public void ThrowExceptionWhenPassingEmptyStringAsFilePath()
        {
            var expectedMsg = new ArgumentException(Resources.Error_InvalidFilePath, "path").Message;

            var exception = Assert.Throws<ArgumentException>(() => new ConfigurationBuilder().AddYamlFile(string.Empty));

            Assert.Equal(expectedMsg, exception.Message);
        }

        [Fact]
        public void YamlConfiguration_Throws_On_Missing_Configuration_File()
        {
            var config = new ConfigurationBuilder().AddYamlFile("NotExistingConfig.Yaml", optional: false);
            var exception = Assert.Throws<FileNotFoundException>(() => config.Build());

            // Assert
            Assert.StartsWith(string.Format(Resources.Error_FileNotFound, "NotExistingConfig.Yaml"), exception.Message);
        }

        [Fact]
        public void YamlConfiguration_Does_Not_Throw_On_Optional_Configuration()
        {
            var config = new ConfigurationBuilder().AddYamlFile("NotExistingConfig.Yaml", optional: true).Build();
        }
    }
}