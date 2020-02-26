namespace NetCoreServer.Models.Fields
{
    public enum RequestType
    {
        Fields,
        Members,
        Select,
        Handshake
    }

    public class FieldsRequest
    {
        public FieldsRequest()
        {
        }

        public FieldsRequest(string index, RequestType type)
        {
            Index = index;
            type = Type;
        }

        public string Index { get; set; }

        public RequestType Type { get; set; }
    }
}