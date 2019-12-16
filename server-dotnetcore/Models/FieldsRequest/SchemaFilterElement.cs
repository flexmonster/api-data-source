namespace NetCoreServer.Models.Fields
{
    public class SchemaFilterElement
    {
        public bool Members { get; set; }
        public bool Query { get; set; }
        public bool ValueQuery { get; set; }

        public SchemaFilterElement()
        {
        }
    }
}