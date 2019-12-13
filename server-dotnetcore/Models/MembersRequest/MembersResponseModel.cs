using DataAPI.Models;
using System.Collections.Generic;

namespace DataAPI.Models.Members
{
    public class MembersResponse
    {
        public MembersResponse()
        {
            Members = new List<Value>();
        }
        public List<Value> Members { get; set; }

        public bool Sorted { get; set; }

        public int Page { get; set; }

        public int PageTotal { get; set; }
    }
}