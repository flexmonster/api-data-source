using NetCoreServer.DataLoaders;
using System.Collections.Generic;

namespace NetCoreServer
{
    public class DatasourceOptions
    {
        public DataSourceType DataSourceName { get; set; }
        public DatabaseType DatabaseType { get; set; }
        public Dictionary<string, string> ConnectionStrings { get; set; }
        public Dictionary<string, string> Indexes { get; set; }
    }
}