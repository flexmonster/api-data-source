using System.Collections.Generic;

namespace NetCoreServer.Models.Fields
{
    public class Schema
    {
        public List<FieldModel> Fields { get; set; }

        public SchemaAggregation Aggregations { get; set; }

        public SchemaFilter Filters { get; set; }

        public bool Sorted { get; set; }

        public Schema()
        {
            Aggregations = new SchemaAggregation();
            Filters = new SchemaFilter();
            Fields = new List<FieldModel>();
        }
    }
}