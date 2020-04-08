using System.Collections.Generic;

namespace NetCoreServer.Models.DataModels
{
    public enum ColumnType
    {
        stringType = 0,
        doubleType,
        dateType
    }

    public class DataColumn<T>
    {
        public ColumnType ColumnType { get; set; }
        public List<T> Values { get; }

        public T this[int index]
        {
            get
            {
                return Values[index];
            }
        }

        public DataColumn(ColumnType type)
        {
            ColumnType = type;
            Values = new List<T>();
        }

        public void Add(T value)
        {
            Values.Add(value);
        }

        public void AddBlock(List<T> column)
        {
            Values.AddRange(column);
        }

        public int Count()
        {
            return Values.Count;
        }
    }
}