using NetCoreServer.Models.DataModels;
using System.Collections.Generic;

namespace NetCoreServer.Models.Fields
{
    public class FieldModel
    {
        public string UniqueName { get; set; }
        public ColumnType Type { get; set; }
        public string Caption { get; set; }
        public string Folder { get; set; }
        public List<string> Aggregations { get; set; }
        public SchemaFilterElement Filter { get; set; }

        public FieldModel()
        {
        }
    }
}