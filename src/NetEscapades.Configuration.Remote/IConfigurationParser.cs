using System.Collections.Generic;
using System.IO;

namespace NetEscapades.Configuration.Remote
{
    public interface IConfigurationParser
    {
        IDictionary<string, string> Parse(Stream input);
    }
}