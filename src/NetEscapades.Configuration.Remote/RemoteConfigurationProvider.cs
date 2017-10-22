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

            if (!string.IsNullOrEmpty(source.ConfigurationKeyPrefix))
            {
                if (source.ConfigurationKeyPrefix.Trim().StartsWith(":"))
                {
                    throw new ArgumentException(string.Format(Resource.Error_InvalidStartCharacter, nameof(source.ConfigurationKeyPrefix), ':'));
                }

                if (source.ConfigurationKeyPrefix.Trim().EndsWith(":"))
                {
                    throw new ArgumentException(string.Format(Resource.Error_InvalidEndCharacter, nameof(source.ConfigurationKeyPrefix), ':'));
                }
            }

            Source = source;

            Backchannel = new HttpClient(source.BackchannelHttpHandler ?? new HttpClientHandler());
            Backchannel.DefaultRequestHeaders.UserAgent.ParseAdd("Remote Confiugration Provider");
            Backchannel.Timeout = source.BackchannelTimeout;
            Backchannel.MaxResponseContentBufferSize = 1024 * 1024 * 10; // 10 MB
            if (source.AuthenticationType == AuthenticationType.Basic)
            {
                if (string.IsNullOrEmpty(source.UserName) || string.IsNullOrEmpty(source.Password)) {
                    throw new ArgumentException("UserName or Password can not be null or empty");
                }
                Backchannel.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(
                        System.Text.Encoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", source.UserName, source.Password))));
            }
            else if (source.AuthenticationType == AuthenticationType.BearerToken)
            {
                if (string.IsNullOrEmpty(source.AuthorizationToken)) {
                    throw new ArgumentException("AuthorizationToken can not be null or empty");
                }
                Backchannel.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", source.AuthorizationToken);
            }            

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

            try
            {
                var response = Backchannel.SendAsync(requestMessage)
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
                        var data = Parser.Parse(stream, Source.ConfigurationKeyPrefix?.Trim());
                        Data = Source.Events.DataParsed(data);
                    }
                }
                else if (!Source.Optional)
                {
                    throw new Exception(string.Format(Resource.Error_HttpError, response.StatusCode, response.ReasonPhrase));
                }
            }
            catch (Exception)
            {
                if (!Source.Optional)
                {
                    throw;
                }
            }
        }
    }
}
