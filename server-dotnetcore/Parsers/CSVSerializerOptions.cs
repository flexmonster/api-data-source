namespace NetCoreServer.Parsers
{
    public class CSVSerializerOptions
    {
        private char _fieldEnclosureToken;

        public char FieldEnclosureToken
        {
            get
            {
                return this._fieldEnclosureToken;
            }
            set
            {
                this._fieldEnclosureToken = value;
            }
        }

        private char _fieldSeparator;

        public char FieldSeparator
        {
            get
            {
                return this._fieldSeparator;
            }
            set
            {
                this._fieldSeparator = value;
            }
        }

        public CSVSerializerOptions(char fieldEnclosureToken = '"', char fieldSeparator = ',')
        {
            _fieldEnclosureToken = fieldEnclosureToken;
            _fieldSeparator = fieldSeparator;
        }
    }
}