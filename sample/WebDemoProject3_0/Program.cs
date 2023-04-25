using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetEscapades.Configuration.Remote;

namespace WebDemoProject3_0
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(AddConfiguration)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        
        private static void AddConfiguration(HostBuilderContext context, IConfigurationBuilder builder)
        {
            var path1 = Path.Combine(context.HostingEnvironment.ContentRootPath, "secrets1");
            var path2 = Path.Combine(context.HostingEnvironment.ContentRootPath, "secrets2");
            using var stream = File.OpenRead("appsettings.yml");
            builder
                .AddYamlFile("appsettings.yml", optional: false)
                .AddYamlStream(stream)
                .Build();
            builder
                .AddKubeSecrets(path1, optional: false)
                .AddKubeSecrets(new PhysicalFileProvider(path2), optional: false)
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
                .AddRemoteSource(new RemoteConfigurationSource()
                {
                    ConfigurationUri = new Uri("http://localhost:5001/api/AuthorizeConfiguration"),
                    // Basic Authentication
                    AuthenticationType = AuthenticationType.Basic,
                    UserName = "username",
                    Password = "password",
                    Optional = false

                });
        }
    }
}
