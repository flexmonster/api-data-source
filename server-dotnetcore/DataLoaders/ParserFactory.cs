using NetCoreServer.JsonConverters;
using NetCoreServer.Parsers;
using System;
using System.Data;
using System.Text.Json;

namespace NetCoreServer.DataLoaders
{
    public enum DatabaseType
    {
        mysql = 0,
        mssql,
        postgresql,
        oracle
    }

    public enum DataSourceType
    {
        json = 0,
        csv,
        database
    }

    public class ParserFactory : IDisposable
    {
        private readonly DatasourceOptions _options;
        private readonly DatabaseConnectionFactory _databaseConnectionFactory;
        private IDbConnection _dbConnection;
        private bool _disposed = false;

        /// <summary>
        /// Factory to create specific parser
        /// </summary>
        /// <param name="configuration">Configuration with datasource and other mandatory parameters</param>
        /// <param name="index">Index</param>
        public ParserFactory(DatasourceOptions options)
        {
            _options = options;
            _databaseConnectionFactory = new DatabaseConnectionFactory();
        }

        /// <summary>
        /// Create parser based on Data source type
        /// </summary>
        /// <param name="dataSourceType">Data source type from settings. Can be "json","csv" or "database"</param>
        /// <returns>Created Parser</returns>
        public IParser CreateParser(string index)
        {
            switch (_options.DataSourceName)
            {
                case DataSourceType.json:
                    {
                        JsonSerializerOptions serializerOptions = new JsonSerializerOptions
                        {
                            AllowTrailingCommas = true,
                            Converters = { new DataJsonConverter() }
                        };
                        var path = _options.Indexes[index];
                        return new JSONParser(path, serializerOptions);
                    }
                case DataSourceType.csv:
                    {
                        CSVSerializerOptions serializerOptions = new CSVSerializerOptions(fieldEnclosureToken: '+');
                        var path = _options.Indexes[index];
                        return new CSVParser(path, serializerOptions);
                    }
                case DataSourceType.database:
                    {
                        _dbConnection = _databaseConnectionFactory.GetDbConnection(_options.ConnectionStrings["DefaultConnection"], _options.DatabaseType);
                        _dbConnection.Open();
                        var dbCommand = _dbConnection.CreateCommand();
                        dbCommand.CommandText = _options.Indexes[index];
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