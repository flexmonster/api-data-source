using NetCoreServer.Models.Fields;

namespace NetCoreServer.Models.Select
{
    public class SelectRequest
    {
        public SelectRequest()
        {
        }

        public SelectRequest(string index, RequestType type, Query query, int page)
        {
            Index = index;
            Type = type;
            Query = query;
            Page = page;
        }

        public string Index { get; set; }

        public RequestType Type { get; set; }

        public Query Query { get; set; }

        public int Page { get; set; }
    }
}