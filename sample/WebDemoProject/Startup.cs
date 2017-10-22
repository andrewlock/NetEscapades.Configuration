using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetEscapades.Configuration.Remote;

namespace WebDemoProject
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddYamlFile("appsettings.yml", optional: false)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddRemoteSource(new Uri("http://localhost:5001/api/configuration"))
                .AddRemoteSource(new Uri("http://localhost:5001/api/endpoint/does/not/exist"), optional: true)
                .AddRemoteSource(new Uri("http://localhost:5002/host/does/not/exist"), optional: true)
                .AddRemoteSource(new RemoteConfigurationSource()
                {
                    ConfigurationUri = new Uri("http://localhost:5001/api/configuration"),
                    ConfigurationKeyPrefix = "anextraprefix",
                    Events = new RemoteConfigurationEvents
                    {
                        OnDataParsed = dict =>
                        {
                            var result = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                            foreach (var keyValuePair in dict)
                            {
                                if (!string.IsNullOrEmpty(keyValuePair.Value))
                                {
                                    result[keyValuePair.Key] = keyValuePair.Value + " is augemented!";
                                }
                            }
                            return result;
                        }
                    }
                })
                .AddRemoteSource(new RemoteConfigurationSource() {
                    ConfigurationUri = new Uri("http://localhost:5001/api/AuthorizeConfiguration"),
                    // Basic Authentication
                    AuthenticationType= AuthenticationType.Basic,
                    UserName="username",
                    Password="password",
                    Optional = true

                });
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            // explicitly inject our config here so we can display it in the values controller
            // NOTE: This is not the recommended usage in practice, it is just for display purposes
            // see https://docs.asp.net/en/latest/fundamentals/configuration.html
            services.AddSingleton<IConfiguration>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}
