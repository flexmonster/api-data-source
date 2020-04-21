using NetCoreServer.Models;
using System.Collections.Generic;

namespace NetCoreServer.Models.Members
{
    public class MembersResponse
    {
        public MembersResponse()
        {
            Members = new List<object>();
        }

        public List<object> Members { get; set; }

        public bool Sorted { get; set; }

        public int Page { get; set; }

        public int PageTotal { get; set; }
    }
}