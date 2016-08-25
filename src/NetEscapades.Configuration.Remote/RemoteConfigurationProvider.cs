using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace NetEscapades.Configuration.Remote
{
    public class RemoteConfigurationProvider : ConfigurationProvider
    {
        public RemoteConfigurationProvider(RemoteConfigurationSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            Source = source;

            Backchannel = new HttpClient(source.BackchannelHttpHandler ?? new HttpClientHandler());
            Backchannel.DefaultRequestHeaders.UserAgent.ParseAdd("Remote Confiugration Provider");
            Backchannel.Timeout = source.BackchannelTimeout;
            Backchannel.MaxResponseContentBufferSize = 1024 * 1024 * 10; // 10 MB

            Parser = source.Parser ?? new JsonConfigurationParser();
        }

        public RemoteConfigurationSource Source { get; }
        
        public IConfigurationParser Parser { get; }

        public HttpClient Backchannel { get; }

        /// <summary>
        /// Loads 
        /// </summary>
        public override void Load()
        {

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, Source.ConfigurationUri);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Source.MediaType));

            Source.Events.SendingRequest(requestMessage);

            var response = Backchannel.SendAsync(requestMessage)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            var content = response.Content.ReadAsStringAsync()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            if (response.IsSuccessStatusCode)
            {
                using (var stream = response.Content.ReadAsStreamAsync()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult())
                {
                    Data = Parser.Parse(stream);
                    Source.Events.DataParsed(Data);
                }
            }
            else
            {
                throw new Exception(string.Format(Resource.Error_HttpError, response.StatusCode, response.ReasonPhrase));
            }
        }
    }
}
