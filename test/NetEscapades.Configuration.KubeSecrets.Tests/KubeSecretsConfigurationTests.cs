using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace NetEscapades.Configuration.KubeSecrets.Tests
{
    public class KubeSecretsConfigurationTests
    {
        [Fact]
        public void ThrowsWhenNotOptionalAndNoSecrets()
        {
            Assert.Throws<DirectoryNotFoundException>(
                () => new ConfigurationBuilder()
                    .AddKubeSecrets(new NonExistantDirectoryProvider())
                    .Build());
        }

        [Fact]
        public void ThrowsWhenProviderReturnsNull()
        {
            var provider = new NullFileProvider();
            var error = Assert.Throws<DirectoryNotFoundException>(() => new ConfigurationBuilder().AddKubeSecrets(provider, optional: false).Build());
        }

        [Fact]
        public void DoesNotThrowWhenOptionalAndNoSecrets()
        {
            new ConfigurationBuilder()
                .AddKubeSecrets(new TestFileProvider(), optional: true)
                .Build();
        }

        [Fact]
        public void CanLoadMultipleSecrets()
        {
            var testFileProvider = new TestFileProvider(
                new TestFile("Secret1", "SecretValue1"),
                new TestFile("Secret2", "SecretValue2"));

            var config = new ConfigurationBuilder()
                .AddKubeSecrets(testFileProvider)
                .Build();

            Assert.Equal("SecretValue1", config["Secret1"]);
            Assert.Equal("SecretValue2", config["Secret2"]);
        }

        [Fact]
        public void CanLoadMultipleSecretsWithDirectory()
        {
            var testFileProvider = new TestFileProvider(
                new TestFile("Secret1", "SecretValue1"),
                new TestFile("Secret2", "SecretValue2"),
                new TestFile("directory"));

            var config = new ConfigurationBuilder()
                .AddKubeSecrets(testFileProvider)
                .Build();

            Assert.Equal("SecretValue1", config["Secret1"]);
            Assert.Equal("SecretValue2", config["Secret2"]);
        }

        [Fact]
        public void CanLoadNestedKeys()
        {
            var testFileProvider = new TestFileProvider(
                new TestFile("Secret0__Secret1__Secret2__Key", "SecretValue2"),
                new TestFile("Secret0__Secret1__Key", "SecretValue1"),
                new TestFile("Secret0__Key", "SecretValue0"));

            var config = new ConfigurationBuilder()
                .AddKubeSecrets(testFileProvider)
                .Build();

            Assert.Equal("SecretValue0", config["Secret0:Key"]);
            Assert.Equal("SecretValue1", config["Secret0:Secret1:Key"]);
            Assert.Equal("SecretValue2", config["Secret0:Secret1:Secret2:Key"]);
        }
    }

    class NonExistantDirectoryProvider : IFileProvider
    {
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return new NonExistantDirectory();
        }

        public IFileInfo GetFileInfo(string subpath)
            => throw new NotImplementedException();

        public IChangeToken Watch(string filter)
            => throw new NotImplementedException();
    }

    class NullFileProvider : IFileProvider
    {
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return null;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            return null;
        }

        public IChangeToken Watch(string filter)
        {
            return null;
        }
    }

    class NonExistantDirectory : IDirectoryContents
    {
        public bool Exists => false;

        public IEnumerator<IFileInfo> GetEnumerator()
            => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator()
            => throw new NotImplementedException();
    }

    class TestFileProvider : IFileProvider
    {
        IDirectoryContents _contents;

        public TestFileProvider(params IFileInfo[] files)
        {
            _contents = new TestDirectoryContents(files);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return _contents;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            throw new NotImplementedException();
        }

        public IChangeToken Watch(string filter)
        {
            throw new NotImplementedException();
        }
    }

    class TestDirectoryContents : IDirectoryContents
    {
        List<IFileInfo> _list;

        public TestDirectoryContents(params IFileInfo[] files)
        {
            _list = new List<IFileInfo>(files);
        }

        public bool Exists
        {
            get
            {
                return true;
            }
        }

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    //TODO: Probably need a directory and file type.
    class TestFile : IFileInfo
    {
        private string _name;
        private string _contents;

        public bool Exists
        {
            get
            {
                return true;
            }
        }

        public bool IsDirectory
        {
            get;
        }

        public DateTimeOffset LastModified
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public long Length
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string PhysicalPath
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public TestFile(string name)
        {
            _name = name;
            IsDirectory = true;
        }

        public TestFile(string name, string contents)
        {
            _name = name;
            _contents = contents;
        }

        public Stream CreateReadStream()
        {
            if (IsDirectory)
            {
                throw new InvalidOperationException("Cannot create stream from directory");
            }

            return new MemoryStream(Encoding.UTF8.GetBytes(_contents));
        }
    }
}
