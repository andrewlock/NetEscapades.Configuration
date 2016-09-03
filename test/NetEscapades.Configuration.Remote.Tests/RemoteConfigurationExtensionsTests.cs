using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace NetEscapades.Configuration.Remote
{
    public class RemoteConfigurationExtensionsTests
    {
        [Fact]
        public void AddRemoteSource_ThrowsIfConfigurationUriIsNull()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder();

            // Act and Assert
            var ex = Assert.Throws<ArgumentNullException>(() => configurationBuilder.AddRemoteSource((Uri) null));
            Assert.Equal("configurationUri", ex.ParamName);
        }

        [Fact]
        public void AddRemoteSource_ThrowsIfEventsIsNull()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder();

            // Act and Assert
            var ex = Assert.Throws<ArgumentNullException>(() => configurationBuilder.AddRemoteSource(new Uri("http://localhost"), false, null));
            Assert.Equal("events", ex.ParamName);
        }

        [Fact]
        public void AddRemoteSource_ThrowsIfSourceIsNull()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder();

            // Act and Assert
            var ex = Assert.Throws<ArgumentNullException>(() => configurationBuilder.AddRemoteSource((RemoteConfigurationSource) null));
            Assert.Equal("source", ex.ParamName);
        }
    }
}
