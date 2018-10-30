# NetEscapades.Configuration

[![AzurePipelines](https://dev.azure.com/andrewlock/NetEscapades.Configuration/_apis/build/status/andrewlock.NetEscapades.Configuration?branchName=master)](https://dev.azure.com/andrewlock/NetEscapades.Configuration/_build/latest?definitionId=1)

[![AppVeyor build status][appveyor-badge]](https://ci.appveyor.com/project/andrewlock/netescapades-configuration)

[![NuGet][yaml-nuget-badge]][yaml-nuget] 
[![NuGet][remote-nuget-badge]][remote-nuget] 
[![NuGet][vault-nuget-badge]][vault-nuget] 

[![MyGet][yaml-myget-badge]][yaml-myget]
[![MyGet][remote-myget-badge]][remote-myget]
[![MyGet][vault-myget-badge]][vault-myget]

[appveyor-badge]: https://ci.appveyor.com/api/projects/status/9klf04bxncm2pgm4?svg=true

[yaml-nuget]: https://www.nuget.org/packages/NetEscapades.Configuration.Yaml/
[yaml-nuget-badge]: https://img.shields.io/nuget/v/NetEscapades.Configuration.Yaml.svg?label=NetEscapades.Configuration.Yaml

[remote-nuget]: https://www.nuget.org/packages/NetEscapades.Configuration.Remote/
[remote-nuget-badge]: https://img.shields.io/nuget/v/NetEscapades.Configuration.Remote.svg?label=NetEscapades.Configuration.Remote

[vault-nuget]: https://www.nuget.org/packages/NetEscapades.Configuration.Vault/
[vault-nuget-badge]: https://img.shields.io/nuget/v/NetEscapades.Configuration.Vault.svg?label=NetEscapades.Configuration.Vault

[yaml-myget]: https://www.myget.org/feed/andrewlock-ci/package/nuget/NetEscapades.Configuration.Yaml
[yaml-myget-badge]: https://img.shields.io/myget/andrewlock-ci/v/NetEscapades.Configuration.Yaml.svg?label=MyGet+Yaml
[remote-myget]: https://www.myget.org/feed/andrewlock-ci/package/nuget/NetEscapades.Configuration.Remote
[remote-myget-badge]: https://img.shields.io/myget/andrewlock-ci/v/NetEscapades.Configuration.Remote.svg?label=MyGet+Remote
[vault-myget]: https://www.myget.org/feed/andrewlock-ci/package/nuget/NetEscapades.Configuration.Vault
[vault-myget-badge]: https://img.shields.io/myget/andrewlock-ci/v/NetEscapades.Configuration.Vault.svg?label=MyGet+Vault

Additional configuration providers to use with ASP.NET Core `Microsoft.Extensions.Configuration`.

## YAML configuration provider 

A YAML configuration provider that uses [YamlDotNet](https://github.com/aaubry/YamlDotNet) to load and parse your YAML files.

### Installing 

Install using the [NetEscapades.Configuration.Yaml NuGet package](https://www.nuget.org/packages/NetEscapades.Configuration.Yaml):

```
PM> Install-Package NetEscapades.Configuration.Yaml
```

### Usage 

When you install the package, it should be added to your _csproj_ file. Alternatively, you can add it directly by adding:

```xml
<PackageReference Include="NetEscapades.Configuration.Yaml" Version="1.5.0" />
```

To load a YAML file as part of your config, just load it as part of your normal `ConfigurationBuilder` setup in the `Program` class of your ASP.NET Core app. 

The simplest possible usage that loads a single YAML file called `appsettings.yml` would be:

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder => 
            {
                builder.AddYamlFile("appsettings.yml", optional: false);
            })
            .UseStartup<Startup>()
            .Build();
}
```

A more complete `Startup` class that loads multiple files (overwriting config values) might look more like the following: 

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder => 
            {
                builder
                    .AddYamlFile("appsettings.yml", optional: false)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            })
            .UseStartup<Startup>()
            .Build();
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

**However**, the `Microsoft.Extensions.Configuration` library is case **insensitive**. Attempting to load the provided file would throw an exception on attempting to load, complaining of a duplicate key.


## Remote configuration provider 

A Remote configuration provider that loads configuration from a remote endpoint. 

### Installing 

Install using the [NetEscapades.Configuration.Remote NuGet package](https://www.nuget.org/packages/NetEscapades.Configuration.Remote):

```
PM> Install-Package NetEscapades.Configuration.Remote
```


### Usage 

When you install the package, it should be added to your _csproj_. Alternatively, you can add it directly by adding:


```xml
<PackageReference Include="NetEscapades.Configuration.Remote" Version="1.4.0" />
```

To load a file from a remote configuration source as part of your config, just load it as part of your normal `ConfigurationBuilder` setup in the `Program` class of your ASP.NET Core app. 

The simplest possible usage that loads a single json file from a remote source `http://localhost:5000` would be:

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder => 
            {
                builder.AddRemoteSource(new Uri("http://localhost"), optional: false);
            })
            .UseStartup<Startup>()
            .Build();
}
```

### Additional configuration

There are a number of properties available for configuration on the `RemoteConfigurationSource` which allow customising the call to the remote endpoint:

* `ConfigurationKeyPrefix` - All Keys loaded from the source will be prefixed with this key `"prefix"` and `"prefix:123"` are valid prefixes, so a key loaded as `<"key", "value">` will be added to the configuration as `<"prefix:123:key", <value>"`.
*  `MediaType` - the media type that the remote source will send, by default `"application/json"`
*  `Parser` - an `IConfigurationParser` that will be used to parse the response. A `JsonConfigurationParser` is included which is taken from the *Microsoft.Extensions.Configuration.Json* pacakge [source code](https://github.com/aspnet/Configuration/tree/dev/src/Microsoft.Extensions.Configuration.Json).
  
The `Events` object provides hooks before sending the request using `OnSendingRequest` and after the data has been processed using `OnDataParsed`.

If the remote source does not return a success response, it will throw an exception, unless you set the `Optional` flag to true. 



## HashiCorp Vault configuration provider 

A configuration provider that loads configuration from an instance of HashiCorp Vault. 

### Installing 

Install using the [NetEscapades.Configuration.Vault NuGet package](https://www.nuget.org/packages/NetEscapades.Configuration.Vault):

```
PM> Install-Package NetEscapades.Configuration.Vault
```

### Usage 

When you install the package, it should be added to your _csproj_. Alternatively, you can add it directly by adding:

```xml
<PackageReference Include="NetEscapades.Configuration.Vault" Version="0.6.4" />
```

You can load secrets from a Vault instance as part of your configuration build. You can use any supported authentication method supported by [`VaultSharp`](https://github.com/rajanadar/VaultSharp), which is used under the hood, but an extension method exists for using `AppRole` specifically.

In ASP.NET Core 2.0, add the provider as part of `ConfigureAppConfiguration` in _Program.cs_:

```csharp
public static IWebHost BuildWebHost(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((ctx, builder)=> 
        {
            // build the initial config
            var builtConfig = config.Build();

            builder.AddVaultWithAppRole(
                config["VaultUri"], //The Vault uri with port
                config["RoleId"], // The role_id for the app
                config["SecretId"], // The secret_iId for the app
                config["SecretPath"] // secret paths to load
                );
        })
        .UseStartup<Startup>()
        .Build();
```


To load a secret from Vault, configure the `VaultConfigurationSource` as part of your normal `ConfigurationBuilder` setup in the `Program` class of your ASP.NET Core app. The approach to configure the builder is similar to the approach required for [the Azure Key Vault provider](https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-2.0&tabs=aspnetcore2x), in which you partially build configuration to obtain the secret values like `role_id` and `secret_id`.


As well as the Vault URI and authentication info, you must provide the paths for secrets to load. For secrets using v2 of the KV secret store, these should include the `data` path segment:

```charp
 "/secret/myapp/secret1",       // secret/myapp/secret1 using v1 of secret store, mounted at secret
 "/secret/data/myapp/secret2",  // secret/myapp/secret2 using v2 of secret store, mounted at secret
 "/secret_v1/myapp/secret3",    // secret_v1/myapp/secret3 using v1 of secret store, mounted at secret_v1
```


> VaultSharp currently supports HashiCorp Vault 0.6.4, but due to the limited required APIs, it can work with many different versions.


To load a secret from Vault, configure the `VaultConfigurationSource` as part of your normal `ConfigurationBuilder` setup in the `Program` class of your ASP.NET Core app. The approach to configure the builder is similar to the approach required for [the Azure Key Vault provider](https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-2.0&tabs=aspnetcore2x). 


### Configure Vault to run the sample


The [VaultSample sample app](/sample/VaultSample) demonstrates how to use the Vault configuration provider. However, you wil also need to configure vault. 

The following guide describes how to run Vault in dev mode, create the test secret, add an `AppRole` and configure the required secrets:

1. Start the server in dev mode: 

```bash
vault server -dev -dev-root-token-id="root"
```

Verify the status

```bash
set VAULT_ADDR=http://127.0.0.1:8200
vault status
```

2. Create a secret that we'll retrieve in the sample
 
```bash
vault kv put secret/sampleapp/thesecret foo=world excited=yes testlocation=thevalue
```

3. Enable approle auth method 

```bash
vault auth enable approle
```

4. Create a policy giving access to the secrets required by the sample app

Either loading from an .hcl file (in the project root):

```bash
vault policy write sampleapp sampleapp-pol.hcl
```
or
```bash
cat sampleapp-pol.hcl | vault policy write sampleapp -
```

or using HEREDOC in bash:
```bash
vault policy write sampleapp -<<'EOF'
# Login with AppRole
path "auth/approle/login" {
  capabilities = [ "create", "read" ]
}

# Read test data (v2)
path "secret/data/sampleapp/*" {
  capabilities =  [ "read" ]
}

# Read test data (v1)
path "secret_v1/sampleapp/*" {
  capabilities =  [ "read" ]
}
EOF
```

> **Note** if you are using version 2 of the kv secrets (the default in Vault 0.10.0+), you need to provide permisions to the `secret/data/*` path, rather than just `secret/data`. You will also need to pass this location in the `UseVault` extension method.

5. Create a new role, **sampleapp-role** and attach the policy 

```bash
vault write auth/approle/role/sampleapp-role policies="sampleapp"
```

6. Get the RoleId for the role

```bash
vault read auth/approle/role/sampleapp-role/role-id
```
This will generate a UUID:

```bash
Key        Value
---        -----
role_id    5b391a7a-8a09-5dd6-f76c-a34d65736f1f
```

7. Generate a new Secret ID for the role

```bash
vault write -f auth/approle/role/sampleapp-role/secret-id
```

This will generate a SecretId and a SecretID Accessor:

```bash
Key                   Value
---                   -----
secret_id             3a80eab2-ae81-40e9-072d-853a5af6b2b2
secret_id_accessor    4d0fe8b2-b1ab-6464-0026-d8700efd085b
```

8. Make the RoleId and SecretId available to the app as configuration values. You should inject these into your app in a secure way, for example using environment variables




## Additional Resources

* [ASP.NET Core Configuration Docs](https://docs.asp.net/en/latest/fundamentals/configuration.html)
* [ASP.NET Core GitHub project](https://github.com/aspnet/Configuration/md)
* [Configuration Providers](http://bleedingnedge.azurewebsites.net/2015/10/15/configuration-providers/)


