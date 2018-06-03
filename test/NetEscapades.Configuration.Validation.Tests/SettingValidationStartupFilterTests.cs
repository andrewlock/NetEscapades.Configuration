using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace NetEscapades.Configuration.Validation.Tests
{
    public class SettingValidationStartupFilterTests
    {
        const string ExpectedResponse = "Hello World!";

        [Fact]
        public async Task WhenConfiguredCorrectly_TestServerBuildsAndRuns()
        {
            var url = "https://localhost";
            var port = 443;

            var testServer = BuildTestServer(url, port.ToString());
            var client = testServer.CreateClient();

            var response = await client.GetStringAsync("/");

            Assert.Equal(ExpectedResponse, response);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        public void WhenEmptyUrl_TestServerThrowsSettingsExceptionOnStartup(string url)
        {
            var port = 443;

            Assert.Throws<SettingsValidationException>(() =>
            {
                var testServer = BuildTestServer(url, port.ToString());
            });
        }


        [Theory]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        public void WhenInvalidPort_TestServerThrowsSettingsExceptionOnStartup(int port)
        {
            var url = "https://localhost";

            Assert.Throws<SettingsValidationException>(() =>
            {
                var testServer = BuildTestServer(url, port.ToString());
            });
        }

        [Theory]
        [InlineData("://oops")]
        [InlineData("http://nota url")]
        [InlineData("http:///notaurl.com")]
        [InlineData(".")]
        public void WhenInvalidUrl_TestServerThrowsUriFormatExceptionOnStartup(string url)
        {
            var port = 443;

            Assert.Throws<UriFormatException>(() =>
            {
                var testServer = BuildTestServer(url, port.ToString());
            });
        }

        [Theory]
        [InlineData("")]
        [InlineData("oops")]
        [InlineData("true")]
        public void WhenInvalidType_TestServerThrowsInvalidOperationExceptionOnStartup(string invalidPortType)
        {
            var url = "https://localhost";

            Assert.Throws<InvalidOperationException>(() =>
            {
                var testServer = BuildTestServer(url, invalidPortType);
            });
        }

        private static TestServer BuildTestServer(string url, string port)
        {

            var webHostBuilder = new WebHostBuilder()
                .ConfigureAppConfiguration(builder =>
                    builder.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            {$"{nameof(StronglyTypedModel)}:{nameof(StronglyTypedModel.Url)}", url },
                            {$"{nameof(StronglyTypedModel)}:{nameof(StronglyTypedModel.Port)}", port },
                        }))
                .UseStartup<Startup>();

            return new TestServer(webHostBuilder);
        }

        public class Startup : IStartup
        {
            private readonly IConfiguration _configuration;

            public Startup(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public void Configure(IApplicationBuilder app)
            {
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync(ExpectedResponse);
                });
            }

            public IServiceProvider ConfigureServices(IServiceCollection services)
            {
                services.UseConfigurationValidation();
                services.ConfigureValidatableSetting<StronglyTypedModel>(_configuration.GetSection(nameof(StronglyTypedModel)));

                return services.BuildServiceProvider();
            }
        }
    }
}
