using NetCoreServer.Models.Fields;
using System.Collections.Generic;

namespace NetCoreServer.Models.Select
{
    public class Query
    {
        public Query()
        {
        }

        public List<FieldModel> Fields { get; set; }

        public AggregationRequest Aggs { get; set; }

        public List<Filter> Filter { get; set; }

        public int Limit { get; set; }
    }
}