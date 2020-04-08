namespace NetCoreServer.Models.Fields
{
    public class SchemaFilter
    {
        public SchemaFilterElement Any { get; set; }

        public SchemaFilter()
        {
            Any = new SchemaFilterElement();
        }
    }
}