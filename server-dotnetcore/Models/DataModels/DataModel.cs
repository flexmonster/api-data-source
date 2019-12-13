using DataAPI.Models.Fields;
using System;
using System.Collections.Generic;

namespace DataAPI.Models
{
    public class Data
    {
        /// <summary>
        /// DataValuesByColumn: key - column name, value - data value in this column
        /// Same indexes in different colums represent one object
        /// </summary>
        public Dictionary<string, Value[]> DataValuesByColumn { get; set; }
        public Data()
        {
            DataValuesByColumn = new Dictionary<string, Value[]>();
        }

        /// <summary>
        /// Convert data stored by rows to data stored by columns
        /// </summary>
        /// <param name="dataValues">Data by rows</param>
        public void ToColumnView(List<Dictionary<string, Value>> dataValues, Schema schema)
        {
            DataValuesByColumn = new Dictionary<string, Value[]>();
            if (dataValues != null && dataValues.Count > 0)
            {
                if (schema == null)
                {
                    var firstElement = dataValues[0];
                    foreach (var field in firstElement)
                    {
                        DataValuesByColumn.Add(field.Key, new Value[dataValues.Count]);
                    }
                }
                else
                {
                    foreach (var field in schema.Fields)
                    {
                        DataValuesByColumn.Add(field.Field, new Value[dataValues.Count]);
                    }
                }
                int i = 0;
                foreach (var dataElement in dataValues)
                {
                    foreach (var field in dataElement)
                    {
                        DataValuesByColumn.TryGetValue(field.Key, out Value[] value);
                        if (value != null) value[i] = field.Value;
                    }
                    i++;
                }
            }
            ReplaceNullsWithEmptyValues();
        }
        /// <summary>
        /// Replace null values with empty for futher processing
        /// </summary>
        private void ReplaceNullsWithEmptyValues()
        {
            foreach (var column in DataValuesByColumn)
            {
                int i = 0;
                foreach (var value in column.Value)
                {
                    if (value == null)
                    {
                        DataValuesByColumn[column.Key][i] = new Value("");
                    }
                    i++;
                }
            }
        }


    }

    public static class DateTimeExtensions
    {
        public static double ToUnixTimestamp(this DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalMilliseconds);
        }
    }
}