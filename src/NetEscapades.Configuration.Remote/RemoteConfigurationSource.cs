using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace NetEscapades.Configuration.Remote
{
    public class RemoteConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// The uri to call to fetch 
        /// </summary>
        public Uri ConfigurationUri { get; set; }

        /// <summary>
        /// The HttpMessageHandler used to communicate with remote configuration provider.
        /// </summary>
        public HttpMessageHandler BackchannelHttpHandler { get; set; }
        
        /// <summary>
        /// Gets or sets timeout value in milliseconds for back channel communications with the remote identity provider.
        /// </summary>
        /// <value>
        /// The back channel timeout.
        /// </value>
        public TimeSpan BackchannelTimeout { get; set; } = TimeSpan.FromSeconds(60);

        /// <summary>
        /// Parser for parsing the returned data into the required configuration source
        /// </summary>
        public IConfigurationParser Parser { get; set; }

        /// <summary>
        /// The accept header used to create a MediaTypeWithQualityHeaderValue
        /// </summary>
        public string MediaType { get; set; } = "application/json";

        public RemoteConfigurationEvents Events { get; set; } = new RemoteConfigurationEvents();

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new RemoteConfigurationProvider(this);
        }
    }
}