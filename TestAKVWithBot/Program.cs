using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TestAKVWithBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var builtConfig = config.Build();
                        
                    using var store = new X509Store(StoreName.My);
                    store.Open(OpenFlags.ReadOnly);
                    Console.WriteLine($"Number of certificates {store.Certificates.Count}");
                    var certs = store.Certificates
                        .Find(X509FindType.FindByThumbprint,
                            builtConfig["AzureADCertThumbprint"], false);

                    config.AddAzureKeyVault(
                        $"https://{builtConfig["KeyVaultName"]}.vault.azure.net/",
                        builtConfig["AzureADApplicationId"],
                        certs.OfType<X509Certificate2>().Single());

                    store.Close();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                
        ;
    }
}
