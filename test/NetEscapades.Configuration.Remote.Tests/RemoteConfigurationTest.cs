using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NetEscapades.Configuration.Remote.Tests;
using Newtonsoft.Json;
using Xunit;
using NetEscapades.Configuration.Tests.Common;

namespace NetEscapades.Configuration.Remote
{
    public class RemoteConfigurationTest
    {
        private RemoteConfigurationProvider LoadProvider(string json, HttpStatusCode code = HttpStatusCode.OK, string prefix = null)
        {
            var p = new RemoteConfigurationProvider(
                new RemoteConfigurationSource
                {
                    BackchannelHttpHandler = CreateServer(json, code),
                    ConfigurationUri = new Uri("http://localhost"),
                    ConfigurationKeyPrefix = prefix,
                });
            p.Load();
            return p;
        }

        [Fact]
        public void LoadKeyValuePairsFromValidJson()
        {
            var json = @"
{
    'firstname': 'test',
    'test.last.name': 'last.name',
        'residential.address': {
            'street.name': 'Something street',
            'zipcode': '12345'
        }
}";
            var jsonConfigSrc = LoadProvider(json);

            Assert.Equal("test", jsonConfigSrc.Get("firstname"));
            Assert.Equal("last.name", jsonConfigSrc.Get("test.last.name"));
            Assert.Equal("Something street", jsonConfigSrc.Get("residential.address:STREET.name"));
            Assert.Equal("12345", jsonConfigSrc.Get("residential.address:zipcode"));
        }

        [Fact]
        public void LoadKeyValuePairsWithPrefixFromValidJson()
        {
            var prefix = "theprefix";
            var json = @"
{
    'firstname': 'test',
    'test.last.name': 'last.name',
        'residential.address': {
            'street.name': 'Something street',
            'zipcode': '12345'
        },
     'nickname': 'tesytest',
}";
            var jsonConfigSrc = LoadProvider(json, prefix: prefix);

            Assert.Equal("test", jsonConfigSrc.Get("theprefix:firstname"));
            Assert.Equal("last.name", jsonConfigSrc.Get("theprefix:test.last.name"));
            Assert.Equal("Something street", jsonConfigSrc.Get("theprefix:residential.address:STREET.name"));
            Assert.Equal("12345", jsonConfigSrc.Get("theprefix:residential.address:zipcode"));
            Assert.Equal("tesytest", jsonConfigSrc.Get("theprefix:nickname"));
        }

        [Fact]
        public void LoadKeyValuePairsWithComplexPrefixFromValidJson()
        {
            var prefix = "theprefix:secondPrefix";
            var json = @"
{
    'firstname': 'test',
    'test.last.name': 'last.name',
        'residential.address': {
            'street.name': 'Something street',
            'zipcode': '12345'
        },
     'nickname': 'tesytest',
}";
            var jsonConfigSrc = LoadProvider(json, prefix: prefix);

            Assert.Equal("test", jsonConfigSrc.Get("theprefix:secondPrefix:firstname"));
            Assert.Equal("last.name", jsonConfigSrc.Get("theprefix:secondPrefix:test.last.name"));
            Assert.Equal("Something street", jsonConfigSrc.Get("theprefix:secondPrefix:residential.address:STREET.name"));
            Assert.Equal("12345", jsonConfigSrc.Get("theprefix:secondPrefix:residential.address:zipcode"));
            Assert.Equal("tesytest", jsonConfigSrc.Get("theprefix:secondPrefix:nickname"));
            Assert.Equal("tesytest", jsonConfigSrc.Get("theprefix:secondPrefix:nickname"));
        }

        [Fact]
        public void LoadMethodCanHandleEmptyValue()
        {
            var json = @"
{
    'name': ''
}";
            var jsonConfigSrc = LoadProvider(json);
            Assert.Equal(string.Empty, jsonConfigSrc.Get("name"));
        }

        [Fact]
        public void NonObjectRootIsInvalid()
        {
            var json = @"'test'";

            var exception = Assert.Throws<FormatException>(
                () => LoadProvider(json));

            Assert.NotNull(exception.Message);
        }

        [Fact]
        public void SupportAndIgnoreComments()
        {
            var json = @"/* Comments */
                {/* Comments */
                ""name"": /* Comments */ ""test"",
                ""address"": {
                    ""street"": ""Something street"", /* Comments */
                    ""zipcode"": ""12345""
                }
            }";
            var jsonConfigSrc = LoadProvider(json);
            Assert.Equal("test", jsonConfigSrc.Get("name"));
            Assert.Equal("Something street", jsonConfigSrc.Get("address:street"));
            Assert.Equal("12345", jsonConfigSrc.Get("address:zipcode"));
        }

        [Fact]
        public void ThrowExceptionWhenUnexpectedEndFoundBeforeFinishParsing()
        {
            var json = @"{
                'name': 'test',
                'address': {
                    'street': 'Something street',
                    'zipcode': '12345'
                }
            /* Missing a right brace here*/";
            var exception = Assert.Throws<FormatException>(() => LoadProvider(json));
            Assert.NotNull(exception.Message);
        }
        
        [Fact]
        public void JsonConfiguration_Throws_On_404()
        {
            var config = new ConfigurationBuilder().AddRemoteSource(
                new RemoteConfigurationSource
            {
                BackchannelHttpHandler = CreateServer(string.Empty, HttpStatusCode.NotFound),
                ConfigurationUri = new Uri("http://localhost"),
            });
            var exception = Assert.Throws<Exception>(() => config.Build());

            // Assert
            Assert.StartsWith("Error calling remote configuration endpoint", exception.Message);
        }

        [Fact]
        public void JsonConfiguration_ReturnsEmptyDataWhenOptional_On_404()
        {
            //arrange
            var configBuilder = new ConfigurationBuilder().AddRemoteSource(
                new RemoteConfigurationSource
            {
                BackchannelHttpHandler = CreateServer(string.Empty, HttpStatusCode.NotFound),
                ConfigurationUri = new Uri("http://localhost"),
                Optional = true,
            });

            //act
            var config = configBuilder.Build();

            //assert
            Assert.NotNull(config);
            Assert.Empty(config.AsEnumerable());
        }

        [Fact]
        public void JsonConfiguration_ReturnsEmptyDataWhenOptionalUsingOverload_On_404()
        {
            //arrange
            var configBuilder = new ConfigurationBuilder()
                .AddRemoteSource(new Uri("http://localhost:123"), optional: true);

            //act
            var config = configBuilder.Build();

            //assert
            Assert.NotNull(config);
            Assert.Empty(config.AsEnumerable());
        }

        [Fact]
        public void JsonConfiguration_Throws_On_500()
        {

            var config = new ConfigurationBuilder().AddRemoteSource(
                new RemoteConfigurationSource
                {
                    BackchannelHttpHandler = CreateServer(string.Empty, HttpStatusCode.InternalServerError),
                    ConfigurationUri = new Uri("http://localhost"),
                });
            var exception = Assert.Throws<Exception>(() => config.Build());

            // Assert
            Assert.StartsWith("Error calling remote configuration endpoint", exception.Message);
        }

        [Fact]
        public void JsonConfiguration_ReturnsEmptyDataWhenOptional_On_500()
        {
            //arrange
            var configBuilder = new ConfigurationBuilder().AddRemoteSource(
                new RemoteConfigurationSource
            {
                BackchannelHttpHandler = CreateServer(string.Empty, HttpStatusCode.InternalServerError),
                ConfigurationUri = new Uri("http://localhost"),
                Optional = true,
            });

            //act
            var config = configBuilder.Build();

            //assert
            Assert.NotNull(config);
            Assert.Empty(config.AsEnumerable());
        }

        [Fact]
        public void ThrowFormatExceptionWhenFileIsEmpty()
        {
            var exception = Assert.Throws<FormatException>(() => LoadProvider(@""));
        }


        [Fact]
        public void AddRemoteSource_ThrowsIfConfigurationPrefixEndsWithColon()
        {
            // Arrange
            var source = new RemoteConfigurationSource
            {
                ConfigurationUri = new Uri("http://localhost"),
                ConfigurationKeyPrefix = "test:",
            };

            // Act and Assert
            Assert.Throws<ArgumentException>(() => new RemoteConfigurationProvider(source));
        }

        [Fact]
        public void AddRemoteSource_ThrowsIfConfigurationPrefixEndsWithColonAndSpace()
        {
            // Arrange
            var source = new RemoteConfigurationSource
            {
                ConfigurationUri = new Uri("http://localhost"),
                ConfigurationKeyPrefix = "test: ",
            };

            // Act and Assert
            Assert.Throws<ArgumentException>(() => new RemoteConfigurationProvider(source));
        }

        [Fact]
        public void AddRemoteSource_ThrowsIfConfigurationPrefixStartsWithColonAndSpace()
        {
            // Arrange
            var source = new RemoteConfigurationSource
            {
                ConfigurationUri = new Uri("http://localhost"),
                ConfigurationKeyPrefix = " :test",
            };

            // Act and Assert
            Assert.Throws<ArgumentException>(() => new RemoteConfigurationProvider(source));
        }

        [Fact]
        public void AddRemoteSource_ThrowsIfConfigurationPrefixStartsWithColon()
        {
            // Arrange
            var source = new RemoteConfigurationSource
            {
                ConfigurationUri = new Uri("http://localhost"),
                ConfigurationKeyPrefix = ":test",
            };

            // Act and Assert
            Assert.Throws<ArgumentException>(() => new RemoteConfigurationProvider(source));
        }

        [Fact]
        public void AddRemoteSource_InvalidCredentials() {
            // Arrange
            var source = new RemoteConfigurationSource
            {
                ConfigurationUri = new Uri("http://localhost"),
                AuthenticationType = AuthenticationType.Basic
            };

            // Act and Assert
            ArgumentException ex = Assert.Throws<ArgumentException>(() => new RemoteConfigurationProvider(source));
            Assert.Equal("UserName or Password can not be null or empty", ex.Message);
        }

        [Fact]
        public void AddRemoteSource_InvalidToken()
        {
            // Arrange
            var source = new RemoteConfigurationSource
            {
                ConfigurationUri = new Uri("http://localhost"),
                AuthenticationType = AuthenticationType.BearerToken
            };

            // Act and Assert
            ArgumentException ex = Assert.Throws<ArgumentException>(() => new RemoteConfigurationProvider(source));
            Assert.Equal("AuthorizationToken can not be null or empty", ex.Message);
        }

        private TestHttpMessageHandler CreateServer(object responseObject, HttpStatusCode code = HttpStatusCode.OK)
        {
            return new TestHttpMessageHandler
            {
                Sender = req => ReturnJsonResponse(responseObject, code)
            };
        }

        private static HttpResponseMessage ReturnJsonResponse(object content, HttpStatusCode code = HttpStatusCode.OK)
        {
            var res = new HttpResponseMessage(code);
            var text = content as string ?? JsonConvert.SerializeObject(content);
            res.Content = new StringContent(text, Encoding.UTF8, "application/json");
            return res;
        }
    }
}
