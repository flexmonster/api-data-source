using System;
using System.Collections.Generic;
using System.Data;
using NetCoreServer.Models;

namespace NetCoreServer.Parsers
{
    public class DatabaseParser : IParser
    {
        private const ushort CHUNK_SIZE = ushort.MaxValue;
        private readonly IDataReader _dataReader;


        private Dictionary<int, string> _columnNames;
        private Dictionary<int, KeyValuePair<Type, Func<object, Value>>> _columnTypesConvertion;

        private Dictionary<string, List<Value>> _dataBlock;
        /// <summary>
        /// Parse database values
        /// </summary>
        /// <param name="dataReader">Datareader to access data in database</param>
        public DatabaseParser(IDataReader dataReader)
        {
            _dataReader = dataReader;
            _columnNames = new Dictionary<int, string>();
            _columnTypesConvertion = new Dictionary<int, KeyValuePair<Type, Func<object, Value>>>();
        }
        public IEnumerable<Dictionary<string, List<Value>>> Parse()
        {
            _dataBlock = new Dictionary<string, List<Value>>();
            object[][] readingChunk = new object[CHUNK_SIZE][];
            int chunckPosition = 0;
            for (int i = 0; i < _dataReader.FieldCount; i++)
            {
                var columnName = _dataReader.GetName(i);
                var columnType = _dataReader.GetFieldType(i);
                _columnTypesConvertion.Add(i, new KeyValuePair<Type, Func<object, Value>>(columnType, DetectType(columnType)));
                _columnNames.Add(i, columnName);
                _dataBlock.Add(columnName, new List<Value>());
            }
            while (_dataReader.Read())
            {
                object[] values = new object[_dataReader.FieldCount];
                _dataReader.GetValues(values);

                readingChunk[chunckPosition] = values;
                chunckPosition++;
                if (chunckPosition == CHUNK_SIZE)
                {
                    ParseBlock(readingChunk);
                    readingChunk = new object[CHUNK_SIZE][];
                    chunckPosition = 0;
                    yield return _dataBlock;
                    foreach (var columnName in _columnNames)
                    {
                        _dataBlock[columnName.Value] = new List<Value>();
                    }
                }
            }
            _dataReader.Close();
            _dataReader.Dispose();
            ParseBlock(readingChunk);

            yield return _dataBlock;
        }
        /// <summary>
        /// Detect type of column based of type returned from database
        /// </summary>
        /// <param name="columnType">type of column</param>
        /// <returns>Function to convert data to specific type</returns>
        private Func<object, Value> DetectType(Type columnType)
        {
            switch (Type.GetTypeCode(columnType))
            {
                case TypeCode.DateTime:
                    return (value) =>
                    {
                        if (!(value is DBNull))
                            return new Value(Convert.ToDateTime(value));
                        return new Value("");
                    };
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return (value) =>
                    {
                        if (!(value is DBNull))
                            return new Value(Convert.ToDouble(value));
                        return new Value("");
                    };
                default:
                    return (value) => { return new Value(Convert.ToString(value)); };
            }
        }

        /// <summary>
        /// Parse rows readed from database
        /// </summary>
        /// <param name="rows"></param>
        private void ParseBlock(object[][] rows)
        {
            var enumerator = rows.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current != null)
                    ParseRow(enumerator.Current as object[]);
            }
        }
        /// <summary>
        /// Parse specific row
        /// </summary>
        /// <param name="values"></param>
        private void ParseRow(object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                _dataBlock[_columnNames[i]].Add(_columnTypesConvertion[i].Value.Invoke(values[i]));
            }
        }
    }
  
}
