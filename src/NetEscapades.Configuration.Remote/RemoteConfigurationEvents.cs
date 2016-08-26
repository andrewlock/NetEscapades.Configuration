using System;
using System.Collections.Generic;
using System.Net.Http;

namespace NetEscapades.Configuration.Remote
{
    public class RemoteConfigurationEvents
    {
        /// <summary>
        /// Called before the <see cref="HttpRequestMessage" /> is sent to allow customising the request
        /// </summary>
        public Action<HttpRequestMessage> OnSendingRequest { get; set; } = msg => { };

        /// <summary>
        /// Called after the data has been parsed allows complete replacement of the data returned.
        /// Should return a case insensitive dictionary using <see cref="StringComparer.OrdinalIgnoreCase" />
        /// </summary>
        public Func<IDictionary<string, string>, IDictionary<string, string>> OnDataParsed { get; set; } = data => data;

        /// <summary>
        /// Called before the <see cref="HttpRequestMessage" /> is sent to allow customising the request
        /// </summary>
        public virtual void SendingRequest(HttpRequestMessage msg) => OnSendingRequest(msg);

        /// <summary>
        /// Called after the data has been parsed allows complete replacement of the data returned.
        /// Should return a case insensitive dictionary using <see cref="StringComparer.OrdinalIgnoreCase" />
        /// </summary>
        public virtual IDictionary<string, string> DataParsed(IDictionary<string, string> data) => OnDataParsed(data);
    }
}