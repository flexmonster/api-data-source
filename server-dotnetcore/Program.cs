using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Runtime.InteropServices;
using System.Reflection;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.Extensions.Logging;

namespace NetCoreServer
{
    public class Program
    {
        private static readonly Dictionary<string, string> DEFAULTS = new Dictionary<string, string>
            {
                { "settings", "appsettings.json" },
                { "port", "3400" }
            };
        private static readonly Dictionary<string, string> ARGS_MAPPING = new Dictionary<string, string>
            {
                { "-s", "settings" },
                { "-p", "port" }
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
            var hostBuilder = new HostBuilder();

            hostBuilder.UseContentRoot(Directory.GetCurrentDirectory());
            hostBuilder.ConfigureHostConfiguration(config =>
            {
                config.AddEnvironmentVariables(prefix: "DOTNET_");
                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            });

            hostBuilder.ConfigureAppConfiguration((hostingContext, configBuilder) =>
            {
                IHostEnvironment env = hostingContext.HostingEnvironment;
                bool reloadOnChange = hostingContext.Configuration.GetValue("hostBuilder:reloadConfigOnChange", defaultValue: true);
                configBuilder.AddConfiguration(configuration);
                configBuilder.AddEnvironmentVariables();
                if (args != null)
                {
                    configBuilder.AddCommandLine(args);
                }
            })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseUrls("http://0.0.0.0:" + configuration.GetValue<string>("port"));
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

                    if (isWindows)
                    {
                        logging.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Warning);
                    }

                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddEventSourceLogger();

                    if (isWindows)
                    {
                        logging.AddEventLog();
                    }
                })
            .UseDefaultServiceProvider((context, options) =>
            {
                bool isDevelopment = context.HostingEnvironment.IsDevelopment();
                options.ValidateScopes = isDevelopment;
                options.ValidateOnBuild = isDevelopment;
            });
            return hostBuilder;
        }
    }
}
