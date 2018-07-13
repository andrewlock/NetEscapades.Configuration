using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Xunit;

namespace NetEscapades.Configuration.Validation.Tests
{
    public class DataAnnotationsSettingValidationStartupFilterTests
    {
        const string ExpectedResponse = "Hello World!";

        [Fact]
        public async Task WhenConfiguredCorrectly_TestServerBuildsAndRuns()
        {
            var url = "https://test.com";
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

            Assert.Throws<ValidationException>(() =>
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

            Assert.Throws<ValidationException>(() =>
            {
                var testServer = BuildTestServer(url, port.ToString());
            });
        }

        [Theory]
        [InlineData("://oops")]
        [InlineData(".")]
        public void WhenInvalidUrl_TestServerThrowsUriFormatExceptionOnStartup(string url)
        {
            var port = 443;

            Assert.Throws<ValidationException>(() =>
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
                            {$"{nameof(ValidateObjectModel)}:{nameof(ValidateObjectModel.Url)}", url },
                            {$"{nameof(ValidateObjectModel)}:{nameof(ValidateObjectModel.Port)}", port },
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
                services.ConfigureValidatableSetting<ValidateObjectModel>(_configuration.GetSection(nameof(ValidateObjectModel)));

                return services.BuildServiceProvider();
            }
        }
    }
}
