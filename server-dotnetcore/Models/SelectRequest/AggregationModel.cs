using NetCoreServer.Models;
using System.Collections.Generic;

namespace NetCoreServer.Models.Select
{
    public class Aggregation
    {
        public Aggregation()
        {
        }

        public Dictionary<string, Dictionary<string, double>> Values { get; set; }

        public Dictionary<string, dynamic> Keys { get; set; }
    }
}