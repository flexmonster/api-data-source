using NetCoreServer.Models.Fields;
using NetCoreServer.Models.Select;
using System.Collections.Generic;

namespace NetCoreServer.Models.Members
{
    public class MembersRequest
    {
        public MembersRequest()
        {
        }

        public MembersRequest(string index, RequestType type, FieldModel field, int page)
        {
            Index = index;
            Type = type;
            Field = field;
            Page = page;
        }

        public string Index { get; set; }

        public RequestType Type { get; set; }

        public FieldModel Field { get; set; }

        public int Page { get; set; }
    }
}