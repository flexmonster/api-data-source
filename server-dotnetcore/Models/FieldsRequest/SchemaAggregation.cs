using System.Collections.Generic;

namespace NetCoreServer.Models.Fields
{
    public class SchemaAggregation
    {
        public List<string> Any { get; set; }

        public List<string> Date { get; set; }

        public List<string> Number { get; set; }

        public List<string> String { get; set; }

        public SchemaAggregation()
        {
        }
    }
}