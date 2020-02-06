using System.Collections.Generic;
using System.Linq;

namespace NetCoreServer.Models.DataModels
{
    public enum Month
    {
        January,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    }

    public enum ShortMonth
    {
        Jan,
        Feb,
        Mar,
        Apr,
        May,
        Jun,
        Jul,
        Aug,
        Sep,
        Oct,
        Nov,
        Dec
    }
    public class ColumnListDataStructure : IDataStructure
    {
        public Dictionary<string, List<Value>> DataValuesByColumn { get; set; }
        public ColumnListDataStructure()
        {
            DataValuesByColumn = new Dictionary<string, List<Value>>();
        }

        public void Add(Dictionary<string, Value> row)
        {
            foreach (var columnName in row.Keys)
            {
                DataValuesByColumn[columnName].Add(row[columnName]);
            }
        }

        public void AddBlock(Dictionary<string, List<Value>> dataBlock)
        {
            foreach (var columnName in dataBlock.Keys)
            {
                DataValuesByColumn[columnName].AddRange(dataBlock[columnName]);
            }
        }

        public int Count()
        {
            if (DataValuesByColumn.Count == 0)
                return 0;
            return DataValuesByColumn.First().Value.Count;
        }

        public Value GetValue(string columnName, int index)
        {
            return DataValuesByColumn[columnName][index];
        }

        public List<Value> GetColumn(string columnName)
        {
            return DataValuesByColumn[columnName];
        }

        public List<string> GetColumnNames()
        {
            return DataValuesByColumn.Keys.ToList();
        }

        public Dictionary<string, Value> GetRow(int position)
        {
            Dictionary<string, Value> row = new Dictionary<string, Value>();
            foreach (var column in DataValuesByColumn)
            {
                row.Add(column.Key, column.Value[position]);
            }
            return row;
        }

        public void AddColumns(List<string> columnNames)
        {
            foreach (var columnName in columnNames)
            {
                DataValuesByColumn.Add(columnName, new List<Value>());
            }
        }
    }
}
