using System.Collections.Generic;

namespace DataAPI.Models.Fields
{
    public class FieldModel
    {
        public string Field { get; set; }
        public string Type { get; set; }
        public string Caption { get; set; }
        public string Folder { get; set; }
        public List<string> Aggregations { get; set; }
        public SchemaFilterElement Filter { get; set; }
        public FieldModel()
        {
        }
    }
}