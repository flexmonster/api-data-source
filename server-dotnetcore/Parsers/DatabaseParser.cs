using NetCoreServer.Extensions;
using NetCoreServer.Models;
using NetCoreServer.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Data;

namespace NetCoreServer.Parsers
{
    public class DatabaseParser : IParser
    {
        private const ushort CHUNK_SIZE = ushort.MaxValue;
        private readonly IDataReader _dataReader;

        private Dictionary<int, string> _columnNames;
        private Dictionary<int, Action<object, dynamic>> _columnAddActionMap;

        private Dictionary<string, dynamic> _dataBlock;

        public DatabaseParser(IDataReader dataReader)
        {
            _dataReader = dataReader;
            _columnNames = new Dictionary<int, string>();
            _dataTypes = new Dictionary<string, ColumnType>();
            _columnAddActionMap = new Dictionary<int, Action<object, dynamic>>();
        }

        private Dictionary<string, ColumnType> _dataTypes;

        public Dictionary<string, ColumnType> DataTypes
        {
            get
            {
                return _dataTypes;
            }
        }

        public IEnumerable<Dictionary<string, dynamic>> Parse()
        {
            _dataBlock = new Dictionary<string, dynamic>();
            object[][] readingChunk = new object[CHUNK_SIZE][];
            int chunckPosition = 0;
            for (int i = 0; i < _dataReader.FieldCount; i++)
            {
                var columnName = _dataReader.GetName(i);
                var columnType = _dataReader.GetFieldType(i);
                _columnAddActionMap.Add(i, DetectType(columnType, out ColumnType dataColumnType));
                _columnNames.Add(i, columnName);
                _dataTypes.Add(columnName, dataColumnType);
                if (dataColumnType == ColumnType.stringType)
                {
                    _dataBlock.Add(columnName, new List<string>());
                }
                else
                {
                    _dataBlock.Add(columnName, new List<double?>());
                }
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
                    foreach (var columnType in _dataTypes)
                    {
                        if (columnType.Value == ColumnType.stringType)
                        {
                            _dataBlock[columnType.Key] = new List<string>();
                        }
                        else
                        {
                            _dataBlock[columnType.Key] = new List<double?>();
                        }
                    }
                }
            }
            _dataReader.Close();
            _dataReader.Dispose();
            ParseBlock(readingChunk);

            yield return _dataBlock;
        }

        private Action<object, dynamic> DetectType(Type columnType, out ColumnType dataColumnType)
        {
            switch (Type.GetTypeCode(columnType))
            {
                case TypeCode.DateTime:
                    {
                        dataColumnType = ColumnType.dateType;
                        return (value, dataColumn) =>
                        {
                            if (value is DBNull)
                                dataColumn.Add((double?)null);
                            else
                                dataColumn.Add(Convert.ToDateTime(value).ToUnixTimestamp());
                        };
                    }
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
                    {
                        dataColumnType = ColumnType.doubleType;
                        return (value, dataColumn) =>
                        {
                            if (value is DBNull)
                                dataColumn.Add((double?)null);
                            else
                                dataColumn.Add(Convert.ToDouble(value));
                        };
                    }
                default:
                    {
                        dataColumnType = ColumnType.stringType;
                        return (value, dataColumn) =>
                        {
                            dataColumn.Add(Convert.ToString(value));
                        };
                    }
            }
        }

        private void ParseBlock(object[][] rows)
        {
            var enumerator = rows.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current != null)
                    ParseRow(enumerator.Current as object[]);
            }
        }

        private void ParseRow(object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                _columnAddActionMap[i].Invoke(values[i], _dataBlock[_columnNames[i]]);
            }
        }
    }
}