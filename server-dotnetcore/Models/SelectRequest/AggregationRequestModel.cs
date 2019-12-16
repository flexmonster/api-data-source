using System.Collections.Generic;

namespace NetCoreServer.Models.Select
{
    public class AggregationRequest
    {
        public AggregationRequest()
        {
        }

        public AggregationBy By { get; set; }

        public List<FieldFuncValue> Values { get; set; }
    }
}