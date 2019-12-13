using DataAPI.Models.Fields;

namespace DataAPI.Models.Select
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