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
using NuGet.Versioning;

namespace NetEscapades.Configuration.Remote
{
    public class RemoteConfigurationTest
    {
        private RemoteConfigurationProvider LoadProvider(string json, HttpStatusCode code = HttpStatusCode.OK)
        {
            var p = new RemoteConfigurationProvider(
                new RemoteConfigurationSource
                {
                    BackchannelHttpHandler = CreateServer(json, code),
                    ConfigurationUri = new Uri("http://localhost"),
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
            Assert.True(exception.Message.StartsWith("Error calling remote configuration endpoint"));
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
            Assert.True(exception.Message.StartsWith("Error calling remote configuration endpoint"));
        }

        [Fact]
        public void ThrowFormatExceptionWhenFileIsEmpty()
        {
            var exception = Assert.Throws<FormatException>(() => LoadProvider(@""));
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
