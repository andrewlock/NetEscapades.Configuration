using System.Collections.Generic;
using System.IO;

namespace NetEscapades.Configuration.Remote
{
    public interface IConfigurationParser
    {
        /// <summary>
        /// Parse the input stream into a configuration dictionary 
        /// </summary>
        /// <param name="input">The stream to parse</param>
        /// <param name="initialContext">The initial context prefix to add to all keys</param>
        /// <returns></returns>
        IDictionary<string, string> Parse(Stream input, string initialContext);
    }
}