using System.Collections.Generic;

namespace NetCoreServer.Models.DataModels
{
    public interface IDataStructure
    {
        void AddColumn(string column, ColumnType type);

        DataColumn<T> GetColumn<T>(string columnName);

        List<string> GetColumnNames();

        public void AddBlock(Dictionary<string, dynamic> dataBlock);

        Dictionary<string, ColumnType> GetNameAndTypes();
    }
}