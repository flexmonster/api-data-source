using DataAPI.Models.Fields;
using System.Collections.Generic;

namespace DataAPI.Models.Select
{
    public class SelectResponse
    {
        public SelectResponse()
        {
            Fields = new List<FieldModel>();
        }
        public List<FieldModel> Fields { get; set; }

        public List<List<Value>> Hits { get; set; }

        public List<Aggregation> Aggs { get; set; }

        public int Page { get; set; }

        public int PageTotal { get; set; }


    }
}