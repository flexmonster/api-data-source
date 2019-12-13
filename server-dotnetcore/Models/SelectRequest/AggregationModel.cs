using DataAPI.Models;
using System.Collections.Generic;

namespace DataAPI.Models.Select
{
    public class Aggregation
    {
        public Aggregation()
        {
        }

        public Dictionary<string, Dictionary<string, double>> Values { get; set; }

        public Dictionary<string, Value> Keys { get; set; }
    }
}