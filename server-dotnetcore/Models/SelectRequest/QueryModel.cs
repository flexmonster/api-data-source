using DataAPI.Models.Fields;
using System.Collections.Generic;

namespace DataAPI.Models.Select
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