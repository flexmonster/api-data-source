using NetCoreServer.JsonConverters;
using NetCoreServer.Parsers;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Text.Json;

namespace NetCoreServer.DataLoaders
{
    public enum DatabaseType
    {
        mysql = 0,
        mssql,
        postgresql
    }
    public enum DataSourceType
    {
        json = 0,
        csv,
        database
    }
    public class ParserFactory : IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly string _index;
        private readonly DatabaseConnectionFactory _databaseConnectionFactory;
        private IDbConnection _dbConnection;
        private bool _disposed = false;

        /// <summary>
        /// Factory to create specific parser 
        /// </summary>
        /// <param name="configuration">Configuration with datasource and other mandatory parameters</param>
        /// <param name="index">Index</param>
        public ParserFactory(IConfiguration configuration, string index)
        {
            _configuration = configuration;
            _index = index;
            _databaseConnectionFactory = new DatabaseConnectionFactory();
        }
        /// <summary>
        /// Create parser based on Data source type
        /// </summary>
        /// <param name="dataSourceType">Data source type from settings. Can be "json","csv" or "database"</param>
        /// <returns>Created Parser</returns>
        public IParser CreateParser(DataSourceType dataSourceType)
        {
            switch (dataSourceType)
            {
                case DataSourceType.json:
                    {
                        JsonSerializerOptions serializerOptions = new JsonSerializerOptions
                        {
                            Converters = { new ValuesJsonConverter() }
                        };
                        return new JSONParser(_index, serializerOptions);
                    }
                case DataSourceType.csv:
                    {
                        CSVSerializerOptions serializerOptions = new CSVSerializerOptions();
                        return new CSVParser(_index, serializerOptions);
                    }
                case DataSourceType.database:
                    {
                        var query = _configuration.GetValue<string>($"DataSource:Indexes:{_index}");
                        var dataBaseType = _configuration.GetValue<DatabaseType>("DataSource:DatabaseType");
                        var connectionString = _configuration.GetValue<string>("DataSource:ConnectionStrings:DefaultConnection");
                        _dbConnection = _databaseConnectionFactory.GetDbConnection(connectionString, dataBaseType);
                        _dbConnection.Open();
                        var dbCommand = _dbConnection.CreateCommand();
                        dbCommand.CommandText = query;
                        dbCommand.Prepare();
                        return new DatabaseParser(dbCommand.ExecuteReader());
                    }
                default:
                    {
                        return null;
                    }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                //dispose connection because used in outside function, but owner is this class
                _dbConnection?.Dispose();
                _disposed = true;
            }

        }

    }
}
