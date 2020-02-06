using System.Collections.Generic;

namespace NetCoreServer.Models.DataModels
{
    public interface IDataStructure
    {
        void Add(Dictionary<string, Value> row);
        void AddBlock(Dictionary<string, List<Value>> dataBlock);
        void AddColumns(List<string> columnNames);
        int Count();
        Value GetValue(string columnName, int index);
        List<Value> GetColumn(string columnName);
        List<string> GetColumnNames();
        Dictionary<string, Value> GetRow(int position);
    }
}
