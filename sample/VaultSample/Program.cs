using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace VaultSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("settings.json");

            var config = builder.Build();

            builder.AddVaultWithAppRole(
                config["VaultUri"],
                config["RoleId"],
                config["SecretId"],
                config["SecretPath"]
                );

            config = builder.Build();

            Console.WriteLine($"foo: {config["foo"]}");
            Console.WriteLine($"excited: {config["excited"]}");
            Console.WriteLine($"testlocation: {config["testlocation"]}");
        }
    }
}
