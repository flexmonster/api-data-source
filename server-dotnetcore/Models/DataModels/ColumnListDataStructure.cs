using NetCoreServer.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetCoreServer.Models
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
        public Dictionary<string, dynamic> DataValuesByColumn { get; set; }

        public ColumnListDataStructure()
        {
            DataValuesByColumn = new Dictionary<string, dynamic>();
        }

        /// <summary>
        /// Get column by name
        /// </summary>
        /// <typeparam name="T">Type of column</typeparam>
        /// <param name="columnName">Column name</param>
        /// <returns></returns>
        public DataColumn<T> GetColumn<T>(string columnName)
        {
            return DataValuesByColumn[columnName];
        }

        /// <summary>
        /// Get all columns names
        /// </summary>
        /// <returns>List of names</returns>
        public List<string> GetColumnNames()
        {
            return DataValuesByColumn.Keys.ToList();
        }

        /// <summary>
        /// Get all columns names and types
        /// </summary>
        /// <returns>Dictionary where key - name, value - type</returns>
        public Dictionary<string, ColumnType> GetNameAndTypes()
        {
            Dictionary<string, ColumnType> row = new Dictionary<string, ColumnType>();
            foreach (var column in DataValuesByColumn)
            {
                row.Add(column.Key, column.Value.ColumnType);
            }
            return row;
        }
        /// <summary>
        /// Add column to DataStructure
        /// </summary>
        /// <param name="columnName">Column name</param>
        /// <param name="type">Column type</param>
        public void AddColumn(string columnName, ColumnType type)
        {
            if (ColumnType.stringType == type)
            {
                DataColumn<string> column = new DataColumn<string>(type);
                DataValuesByColumn.Add(columnName, column);
            }
            else
            {
                DataColumn<double?> column = new DataColumn<double?>(type);
                DataValuesByColumn.Add(columnName, column);
            }
        }

        /// <summary>
        /// Add block of values
        /// </summary>
        /// <param name="dataBlock"></param>
        public void AddBlock(Dictionary<string, dynamic> dataBlock)
        {
            foreach (var column in dataBlock)
            {
                DataValuesByColumn[column.Key].AddBlock(column.Value);
            }
        }
    }
}