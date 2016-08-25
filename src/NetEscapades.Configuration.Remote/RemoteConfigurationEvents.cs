using System;
using System.Collections.Generic;
using System.Net.Http;

namespace NetEscapades.Configuration.Remote
{
    public class RemoteConfigurationEvents
    {
        /// <summary>
        /// Called before the HttpRequestMessage is sent
        /// </summary>
        public Action<HttpRequestMessage> OnSendingRequest { get; set; } = msg => { };

        /// <summary>
        /// Called after the data has been parsed
        /// </summary>
        public Action<IDictionary<string, string>> OnDataParsed { get; set; } = data => { };

        public void SendingRequest(HttpRequestMessage msg) => OnSendingRequest(msg);

        public void DataParsed(IDictionary<string, string> data) => OnDataParsed(data);
    }
}