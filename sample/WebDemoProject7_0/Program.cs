using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using NetEscapades.Configuration.Validation;

var builder = WebApplication.CreateBuilder();

builder.Services.ConfigureValidatableSetting<TestSettings>(builder.Configuration);
builder.Services.UseConfigurationValidation();

var secretsPath1 = Path.Combine(builder.Environment.ContentRootPath, "secrets1");
var secretsPath2 = Path.Combine(builder.Environment.ContentRootPath, "secrets2");
using var stream = File.OpenRead("appsettings.yml");
builder.Configuration
    .AddYamlFile("appsettings.yml", optional: false)
    .AddYamlStream(stream)
    .AddKubeSecrets(secretsPath1, optional: false)
    .AddKubeSecrets(new PhysicalFileProvider(secretsPath2), optional: false);

var app = builder.Build();

app.MapGet("/api/values", (IConfiguration config) => config.AsEnumerable()
    .OrderBy(kvp => kvp.Key)
    .Select(kvp => kvp.Key + " - " + kvp.Value));

app.Run();
public class TestSettings: IValidatable
{
    public void Validate()
    {
        // Uncomment this line to throw on startup
        // throw new ValidationException("Throwing test exception");
    }
}