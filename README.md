# NetEscapades.Configuration

[![Build status](https://ci.appveyor.com/api/projects/status/9klf04bxncm2pgm4?svg=true)](https://ci.appveyor.com/project/andrewlock/netescapades-configuration)
[![Travis](https://img.shields.io/travis/andrewlock/NetEscapades.Configuration.svg?maxAge=3600&label=travis)](https://travis-ci.org/andrewlock/NetEscapades.Configuration)
[![NuGet](https://img.shields.io/nuget/v/NetEscapades.Configuration.Yaml.svg)](https://www.nuget.org/packages/NetEscapades.Configuration.Yaml/)
[![MyGet CI](https://img.shields.io/myget/andrewlock-ci/v/NetEscapades.Configuration.Yaml.svg)](http://myget.org/gallery/andrewlock-ci)

Additional configuration providers to use with ASP.NET Core `Microsoft.Extensions.Configuration`.

## YAML configuration provider 

A YAML configuration provider that uses [YamlDotNet](https://github.com/aaubry/YamlDotNet) to load and parse your YAML files.

### Installing 

Install using the [NetEscapades.Configuration.Yaml NuGet package](https://www.nuget.org/packages/NetEscapades.Configuration.Yaml):

```
PM> Install-Package NetEscapades.Configuration.Yaml
```

###Usage 

When you install the package, it should be added to your `package.json`. Alternatively, you can add it directly by adding:

```json
{
  "dependencies" : {
    "NetEscapades.Configuration.Yaml": "1.0.3"
  }
}
```

To load a YAML file as part of your config, just load it as part of your normal `ConfigurationBuilder` setup in the `Startup` class of your ASP.NET Core app. 

The simplest possible usage that loads a single YAML file called `appsettings.yml` would be:

```csharp
public class Startup
{
  public Startup(IHostingEnvironment env)
  {
    var builder = new ConfigurationBuilder()
      .SetBasePath(env.ContentRootPath)
      .AddYamlFile("appsettings.yml", optional: false);
    Configuration = builder.Build();
  }
  
  public IConfigurationRoot Configuration { get; }
}
```

A more complete `Startup` class that loads multiple files (overwriting config values) might look more like the following: 

```csharp
public class Startup
{
    public Startup(IHostingEnvironment env)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddYamlFile("appsettings.yml", optional: false)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
        Configuration = builder.Build();
    }

    public IConfigurationRoot Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        // Add framework services.
        services.AddMvc();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
        loggerFactory.AddConsole(Configuration.GetSection("Logging"));
        loggerFactory.AddDebug();

        app.UseMvc();
    }
}
```

There is a demo Web API project in the test folder of the GitHub project at https://github.com/andrewlock/NetEscapades.Configuration/tree/master/test/WebDemoProject

### Troubleshooting

One thing to be aware of is that the YAML specification is case **sensitive**, so the following file is valid and has 3 distinct keys:

```yaml
test: Value1
Test: Value2
TEST: Value3
```

**However**, the `Microsoft.Extensions.Configuration` library is case **insensitive**. Attempting to load the provided file would throw an exception on attempting to load, compaining of a duplicate key.

## Additional Resources

* [ASP.NET Core Configuration Docs](https://docs.asp.net/en/latest/fundamentals/configuration.html)
* [ASP.NET Core GitHub project](https://github.com/aspnet/Configuration/md)
* [Configuration Providers](http://bleedingnedge.azurewebsites.net/2015/10/15/configuration-providers/)


