using NetCoreServer.DataLoaders;
using System.Collections.Generic;

namespace NetCoreServer
{
    public class DatasourceOptions
    {
        public Dictionary<string, IndexOptions> Indexes { get; set; }
    }
    public class IndexOptions
    {
        public string Type { get; set; }
        public IndexOptions() { }
        public IndexOptions(string type)
        {
            Type = type;
        }
    }
    public class JsonIndexOptions : IndexOptions
    {
        public string Path { get; set; }
        public JsonIndexOptions() { }
        public JsonIndexOptions(string path)
        {
            Path = path;
        }

    }
    public class CsvIndexOptions : IndexOptions
    {
        public string Path { get; set; }
        public char? Delimiter { get; set; }
        public CsvIndexOptions() { }

        public CsvIndexOptions(string path)
        {
            Path = path;
        }
    }
    public class DatabaseIndexOptions : IndexOptions
    {
        public string Query { get; set; }
        public string DatabaseType { get; set; }
        public string ConnectionString { get; set; }

        public DatabaseIndexOptions() { }
        public DatabaseIndexOptions(string databaseType, string connectionString, string query)
        {
            ConnectionString = connectionString;
            DatabaseType = databaseType;
            Query = query;
        }
    }
}