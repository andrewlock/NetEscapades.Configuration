using System;
using Microsoft.Extensions.Configuration;
using Moq;
using VaultSharp;
using Xunit;

namespace NetEscapades.Configuration.Vault.Tests
{
    public class VaultConfigurationExtensionTest
    {
        [Fact]
        public void ConfigurationFailsIfDuplicatePrefixesDefined()
        {
            var configurationMock = Mock.Of<IConfigurationBuilder>();
            var client = Mock.Of<IVaultClient>();
            var secretManager = Mock.Of<IVaultSecretManager>();
            var mappingWithDuplicate = new []
            {
                new VaultSecretMapping("duplicatePrefix", "someValue1"),
                new VaultSecretMapping("normalPrefix", "someValue2"),
                new VaultSecretMapping("duplicatePrefix", "someValue3")
            };

            Assert.Throws<ArgumentException>(() =>
            {
                configurationMock.AddVault(client, secretManager, false, mappingWithDuplicate);
            });
        }
        
        [Fact]
        public void ConfigurationDoesNotOnDuplicateEmptyPrefixes()
        {
            var configurationMock = Mock.Of<IConfigurationBuilder>();
            var client = Mock.Of<IVaultClient>();
            var secretManager = Mock.Of<IVaultSecretManager>();
            var mappingWithDuplicate = new []
            {
                new VaultSecretMapping("", "someValue1"),
                new VaultSecretMapping(null, "someValue2"),
                new VaultSecretMapping("", "someValue3"),
                new VaultSecretMapping(null, "someValue2"),
            };

            configurationMock.AddVault(client, secretManager, false, mappingWithDuplicate);
        }
        
        [Fact]
        public void ConfigurationFailsIfDependenciesAreNull()
        {
            var configurationMock = Mock.Of<IConfigurationBuilder>();
            var client = Mock.Of<IVaultClient>();
            var secretManager = Mock.Of<IVaultSecretManager>();
            var mappings = new [] { new VaultSecretMapping("prefix", "someValue2") };
            
            Assert.Throws<ArgumentNullException>(() =>
            {
                (null as IConfigurationBuilder).AddVault(client, secretManager, false, mappings);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                configurationMock.AddVault(null, secretManager, false, mappings);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                configurationMock.AddVault(client, null, false, mappings);
            });
        }
        
        [Fact]
        public void ConfigurationFailsIfMappingsAreEmpty()
        {
            var configurationMock = Mock.Of<IConfigurationBuilder>();
            var client = Mock.Of<IVaultClient>();
            var secretManager = Mock.Of<IVaultSecretManager>();
            
            Assert.Throws<ArgumentException>(() =>
            {
                configurationMock.AddVault(client, secretManager, false, Array.Empty<VaultSecretMapping>());
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                configurationMock.AddVault(client, null, false, null as VaultSecretMapping[]);
            });
        }
    }
}