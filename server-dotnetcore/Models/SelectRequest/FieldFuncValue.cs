using NetCoreServer.Models.Fields;

namespace NetCoreServer.Models.Select
{
    public class FieldFuncValue
    {
        public FieldFuncValue()
        {
        }

        public FieldModel Field { get; set; }

        public string Func { get; set; }
    }
}