using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace NetCoreServer
{
    public class Program
    {
        private static readonly Dictionary<string, string> DEFAULTS = new Dictionary<string, string>
            {
                { "settings", "appsettings.json" },
                { "urls", "http://0.0.0.0:3400" }
            };
        private static readonly Dictionary<string, string> ARGS_MAPPING = new Dictionary<string, string>
            {
                { "-s", "settings" },
                { "-u", "urls" }
            };
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            // loads settings from defauls and command line to find the path to the configuration file
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddInMemoryCollection(DEFAULTS)
                .AddCommandLine(args, Program.ARGS_MAPPING);

            var configuration = configBuilder.Build();
            string defaultConfigFilename = configuration.GetValue<string>("settings");

            // loads all settings, now including the configuration file
            configBuilder.Sources.Clear();
            configBuilder.AddInMemoryCollection(DEFAULTS)
                .AddJsonFile(defaultConfigFilename)
                .AddCommandLine(args, Program.ARGS_MAPPING);
            configuration = configBuilder.Build();

            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configBuilder) =>
                {
                    configBuilder.AddConfiguration(configuration);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseUrls(configuration.GetValue<string>("urls"));
                });

            return hostBuilder;
        }
    }
}
