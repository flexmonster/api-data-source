using NetCoreServer.Models.Fields;
using System.Collections.Generic;

namespace NetCoreServer.Models.Select
{
    public class AggregationBy
    {
        public AggregationBy()
        {
            Rows = new List<FieldModel>();
            Cols = new List<FieldModel>();
        }

        public List<FieldModel> Rows { get; set; }

        public List<FieldModel> Cols { get; set; }
    }
}