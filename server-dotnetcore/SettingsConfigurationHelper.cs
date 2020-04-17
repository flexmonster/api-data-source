using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreServer.DataStorages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreServer
{
    public static class SettingsConfigurationHelper
    {
        /// <summary>
        /// Extension method for configuration options from IConfiguration
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureFlexmonsterOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatasourceOptions>((options) =>
            {
                options.Indexes = new Dictionary<string, IndexOptions>();
                foreach (var datasourceConfig in configuration.GetSection("DataSources").GetChildren())
                {
                    if (datasourceConfig.GetSection("Type").Get<string>() == "json")
                    {
                        foreach (var indexConfig in datasourceConfig.GetSection("Indexes").GetChildren())
                        {
                            var jsonConfig = indexConfig.Get<JsonIndexOptions>();
                            jsonConfig.Type = "json";
                            options.Indexes.Add(indexConfig.Key, jsonConfig);
                        }
                    }
                    else if (datasourceConfig.GetSection("Type").Get<string>() == "csv")
                    {
                        foreach (var indexConfig in datasourceConfig.GetSection("Indexes").GetChildren())
                        {
                            var csvConfig = indexConfig.Get<CsvIndexOptions>();
                            csvConfig.Type = "csv";
                            options.Indexes.Add(indexConfig.Key, csvConfig);
                        }
                    }
                    else if (datasourceConfig.GetSection("Type").Get<string>() == "database")
                    {
                        foreach (var indexConfig in datasourceConfig.GetSection("Indexes").GetChildren())
                        {
                            var dbConfig = indexConfig.Get<DatabaseIndexOptions>();
                            dbConfig.ConnectionString = datasourceConfig.GetSection("ConnectionString").Get<string>();
                            dbConfig.DatabaseType = datasourceConfig.GetSection("DatabaseType").Get<string>();
                            dbConfig.Type = "database";
                            options.Indexes.Add(indexConfig.Key, dbConfig);
                        }
                    }
                    else
                    {
                        var indexOption = new IndexOptions() { Type = datasourceConfig.GetSection("Type").Get<string>() };
                        foreach (var indexConfig in datasourceConfig.GetSection("Indexes").GetChildren())
                        {
                            options.Indexes.Add(indexConfig.Key, indexOption);
                        }
                    }
                }
            });
            services.Configure<DataStorageOptions>(configuration.GetSection("DataStorageOptions"));
        }
    }
}
