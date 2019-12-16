using NetCoreServer.Models.Fields;
using System.Collections.Generic;

namespace NetCoreServer.Models.Select
{
    public class Filter
    {
        public FieldModel Field { get; set; }

        public string FieldType { get; set; }

        public List<Value> Include { get; set; }

        public List<Value> Exclude { get; set; }

        public Dictionary<string, Value> Query { get; set; }

        public FieldFuncValue Value { get; set; }

        public Filter()
        {
        }
    }
}