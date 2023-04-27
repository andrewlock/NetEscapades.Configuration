using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace NetEscapades.Configuration.Yaml
{
    internal class StaticConfigurationSource: IConfigurationSource
    {
        public IDictionary<string,string> Data { get; set; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
            => new StaticConfigurationProvider(Data);
    }
}