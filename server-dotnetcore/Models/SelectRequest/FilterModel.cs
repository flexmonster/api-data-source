using NetCoreServer.Models.Fields;
using System.Collections.Generic;

namespace NetCoreServer.Models.Select
{
    public class Filter
    {
        public FieldModel Field { get; set; }

        public string FieldType { get; set; }

        public List<HierarchyObject> Include { get; set; }

        public List<HierarchyObject> Exclude { get; set; }

        public Dictionary<string, ValueObject> Query { get; set; }

        public FieldFuncValue Value { get; set; }

        public Filter()
        {
        }
    }
}