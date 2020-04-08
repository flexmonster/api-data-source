using NetCoreServer.Models.Fields;

namespace NetCoreServer.Models.Handshake
{
    public class HandshakeRequst
    {
        public string Version { get; set; }
        public RequestType Type { get; set; }
    }
}