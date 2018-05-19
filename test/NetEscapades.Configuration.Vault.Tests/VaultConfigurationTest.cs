using Moq;
using NetEscapades.Configuration.Tests.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaultSharp;
using VaultSharp.Backends.System.Models;
using Xunit;

namespace NetEscapades.Configuration.Vault.Tests
{
    public class VaultConfigurationTest
    {
        private const string SecretPath = "/secrets/Development/testapp";
        private const string DataKey = "data";
        private const string MetaDataKey = "metadata";

        [Fact]
        public void LoadsAllSecretsFromVault()
        {
            var client = new Mock<IVaultClient>(MockBehavior.Strict);
            var secret1Id = GetSecretId("Secret1");
            var secret2Id = GetSecretId("Secret2");

            client.Setup(c => c.ReadSecretAsync(SecretPath)).ReturnsAsync(new Secret<Dictionary<string, object>>
            {
                Data = new Dictionary<string, object> {
                    { secret1Id, "Value1" },
                    { secret2Id, "Value2" },
                }
            });

            // Act
            var provider = new VaultConfigurationProvider(client.Object, new DefaultVaultSecretManager(), new[] { SecretPath });
            provider.Load();

            // Assert
            client.VerifyAll();

            var childKeys = provider.GetChildKeys(Enumerable.Empty<string>(), null).ToArray();
            Assert.Equal(new[] { "Secret1", "Secret2" }, childKeys);
            Assert.Equal("Value1", provider.Get("Secret1"));
            Assert.Equal("Value2", provider.Get("Secret2"));
        }

        [Fact]
        public void LoadsAllSecretsFromVaultIfLooksLikeV2Data()
        {
            var client = new Mock<IVaultClient>(MockBehavior.Strict);
            var secret1Id = GetSecretId("Secret1");
            var secret2Id = GetSecretId("Secret2");

            client.Setup(c => c.ReadSecretAsync(SecretPath)).ReturnsAsync(new Secret<Dictionary<string, object>>
            {
                Data = new Dictionary<string, object> {
                    {DataKey, new JObject {
                        [secret1Id] =  "Value1",
                        [secret2Id] =  "Value2",
                    }},
                    {MetaDataKey, "" }
                }
            });

            // Act
            var provider = new VaultConfigurationProvider(client.Object, new DefaultVaultSecretManager(), new[] { SecretPath });
            provider.Load();

            // Assert
            client.VerifyAll();

            var childKeys = provider.GetChildKeys(Enumerable.Empty<string>(), null).ToArray();
            Assert.Equal(new[] { "Secret1", "Secret2" }, childKeys);
            Assert.Equal("Value1", provider.Get("Secret1"));
            Assert.Equal("Value2", provider.Get("Secret2"));
        }

        [Fact]
        public void DoesNotLoadFilteredItems()
        {
            var client = new Mock<IVaultClient>(MockBehavior.Strict);
            var secret1Id = GetSecretId("Secret1");
            var secret2Id = GetSecretId("Secret2");

            client.Setup(c => c.ReadSecretAsync(SecretPath)).ReturnsAsync(new Secret<Dictionary<string, object>>
            {
                Data = new Dictionary<string, object> {
                    { secret1Id, "Value1" },
                    { secret2Id, "Value2" },
                }
            });

            // Act
            var provider = new VaultConfigurationProvider(client.Object, new EndsWithOneVaultSecretManager(), new[] { SecretPath });
            provider.Load();

            // Assert
            client.VerifyAll();

            var childKeys = provider.GetChildKeys(Enumerable.Empty<string>(), null).ToArray();
            Assert.Equal(new[] { "Secret1" }, childKeys);
            Assert.Equal("Value1", provider.Get("Secret1"));
        }

        [Fact]
        public void SupportsReload()
        {
            var client = new Mock<IVaultClient>(MockBehavior.Strict);
            var secret1Id = GetSecretId("Secret1");
            var value = "Value1";

            client.Setup(c => c.ReadSecretAsync(SecretPath)).Returns((string path) => Task.FromResult(new Secret<Dictionary<string, object>>
            {
                Data = new Dictionary<string, object> {
                    { secret1Id, value },
                }
            }));

            // Act & Assert
            var provider = new VaultConfigurationProvider(client.Object, new DefaultVaultSecretManager(), new[] { SecretPath });
            provider.Load();

            client.VerifyAll();
            Assert.Equal("Value1", provider.Get("Secret1"));

            value = "Value2";
            provider.Load();
            Assert.Equal("Value2", provider.Get("Secret1"));
        }

        [Fact]
        public void PreservesColonInSecretName()
        {
            var client = new Mock<IVaultClient>(MockBehavior.Strict);
            var secret1Id = GetSecretId("Section:Secret1");

            client.Setup(c => c.ReadSecretAsync(SecretPath)).ReturnsAsync(new Secret<Dictionary<string, object>>
            {
                Data = new Dictionary<string, object> {
                    { secret1Id, "Value1" },
                }
            });

            // Act
            var provider = new VaultConfigurationProvider(client.Object, new DefaultVaultSecretManager(), new[] { SecretPath });
            provider.Load();

            // Assert
            client.VerifyAll();

            Assert.Equal("Value1", provider.Get("Section:Secret1"));
        }

        [Fact]
        public void ConstructorThrowsForNullManager()
        {
            Assert.Throws<ArgumentNullException>(() => new VaultConfigurationProvider(Mock.Of<IVaultClient>(), null, new[] { SecretPath }));
        }

        private string GetSecretId(string name) => name;

        private class EndsWithOneVaultSecretManager : DefaultVaultSecretManager
        {
            public override bool Load(Secret<Dictionary<string, object>> secret, string key)
            {
                return key.EndsWith("1");
            }
        }

    }
}
