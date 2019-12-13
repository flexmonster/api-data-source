using DataAPI.Models.Fields;

namespace DataAPI.Models.Handshake
{
    public class HandshakeRequst
    {
        public string Version { get; set; }
        public RequestType Type { get; set; }
    }
}
