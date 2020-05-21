using NetCoreServer.JsonConverters;
using NetCoreServer.Parsers;
using System;
using System.Data;
using System.Text.Json;

namespace NetCoreServer.DataLoaders
{

    public class ParserFactory : IDisposable
    {
        private readonly IndexOptions _options;
        private readonly DatabaseConnectionFactory _databaseConnectionFactory;
        private IDbConnection _dbConnection;
        private bool _disposed = false;

        /// <summary>
        /// Factory to create specific parser
        /// </summary>
        /// <param name="configuration">Configuration with datasource and other mandatory parameters</param>
        /// <param name="index">Index</param>
        public ParserFactory(IndexOptions options)
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
            switch (_options.Type)
            {
                case "json":
                    {
                        var path = (_options as JsonIndexOptions)?.Path;
                        JsonSerializerOptions serializerOptions = new JsonSerializerOptions
                        {
                            AllowTrailingCommas = true,
                            Converters = { new DataJsonConverter() }
                        };
                        return new JSONParser(path, serializerOptions);
                    }
                case "csv":
                    {
                        var csvOptions = _options as CsvIndexOptions;
                        var path = csvOptions?.Path;
                        var delimeter = csvOptions?.Delimiter;
                        CSVSerializerOptions serializerOptions = new CSVSerializerOptions();
                        if (delimeter.HasValue)
                        {
                            serializerOptions.FieldSeparator = delimeter.Value;
                        }
                        return new CSVParser(path, serializerOptions);
                    }
                case "database":
                    {
                        var options = _options as DatabaseIndexOptions;
                        _dbConnection = _databaseConnectionFactory.GetDbConnection(options.ConnectionString, options.DatabaseType);
                        _dbConnection.Open();
                        var dbCommand = _dbConnection.CreateCommand();
                        dbCommand.CommandText = options?.Query;
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