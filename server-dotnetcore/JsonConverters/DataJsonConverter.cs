using NetCoreServer.Extensions;
using NetCoreServer.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NetCoreServer.JsonConverters
{
    /// <summary>
    /// Converter from JSON to ColumnDataStructure
    /// </summary>
    public class DataJsonConverter : JsonConverter<Dictionary<string, dynamic>>
    {
        private Dictionary<string, ColumnType> _columnTypes;

        public Dictionary<string, ColumnType> GetTypes()
        {
            return _columnTypes;
        }

        public override Dictionary<string, dynamic> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.Read();
            Dictionary<string, dynamic> columnList = new Dictionary<string, dynamic>();
            if (_columnTypes == null)
            {
                _columnTypes = new Dictionary<string, ColumnType>();
                reader.Read();
                while (reader.TokenType != JsonTokenType.EndObject)
                {
                    var prop = reader.GetString();
                    reader.Read();
                    if (reader.TokenType == JsonTokenType.String)
                    {
                        var value = reader.GetString();
                        reader.Read();
                        if (DateTime.TryParse(value, out DateTime dateValue))
                        {
                            columnList.Add(prop, new List<double?>());
                            columnList[prop].Add(dateValue.ToUnixTimestamp());
                            _columnTypes.Add(prop, ColumnType.dateType);
                        }
                        else
                        {
                            columnList.Add(prop, new List<string>());
                            columnList[prop].Add(value);
                            _columnTypes.Add(prop, ColumnType.stringType);
                        }
                    }
                    else if (reader.TokenType == JsonTokenType.Number)
                    {
                        var value = reader.GetDouble();
                        reader.Read();
                        columnList.Add(prop, new List<double?>());
                        columnList[prop].Add(value);
                        _columnTypes.Add(prop, ColumnType.doubleType);
                    }
                }
                reader.Read();
            }
            else
            {
                foreach (var column in _columnTypes)
                {
                    if (column.Value == ColumnType.stringType)
                    {
                        columnList.Add(column.Key, new List<string>());
                    }
                    else
                    {
                        columnList.Add(column.Key, new List<double?>());
                    }
                }
            }
            while (reader.TokenType != JsonTokenType.EndArray)
            {
                reader.Read();
                int propertyCount = 0;
                while (reader.TokenType != JsonTokenType.EndObject)
                {
                    var prop = reader.GetString();
                    reader.Read();
                    if (_columnTypes.ContainsKey(prop))
                    {
                        if (_columnTypes[prop] == ColumnType.stringType)
                        {
                            if (reader.TokenType == JsonTokenType.Number)
                            {
                                var value = reader.GetDouble();
                                reader.Read();
                                columnList[prop].Add(value.ToString());
                            }
                            else
                            {
                                var value = reader.GetString();
                                reader.Read();
                                if (value != "")
                                {
                                    columnList[prop].Add(value);
                                }
                                else
                                {
                                    columnList[prop].Add((string)null);
                                }
                            }
                        }
                        else if (_columnTypes[prop] == ColumnType.doubleType)
                        {
                            if (reader.TokenType == JsonTokenType.Number)
                            {
                                var value = reader.GetDouble();
                                reader.Read();
                                columnList[prop].Add(value);
                            }
                            else
                            {
                                var value = reader.GetString();
                                reader.Read();
                                columnList[prop].Add((double?)null);
                            }
                        }
                        else if (_columnTypes[prop] == ColumnType.dateType)
                        {
                            var value = reader.GetString();
                            reader.Read();
                            if (DateTime.TryParse(value, out DateTime dateValue))
                            {
                                columnList[prop].Add(dateValue.ToUnixTimestamp());
                            }
                            else
                            {
                                columnList[prop].Add((double?)null);
                            }
                        }
                        propertyCount++;
                    }
                    else
                    {
                        reader.Read();
                    }
                }
                if (propertyCount < _columnTypes.Count)
                {
                    var currentCount = columnList.Max(column => column.Value.Count);
                    foreach (var column in columnList)
                    {
                        if (column.Value.Count < currentCount)
                        {
                            if (_columnTypes[column.Key] == ColumnType.stringType)
                            {
                                column.Value.Add((string)null);
                            }
                            else
                            {
                                column.Value.Add((double?)null);
                            }
                        }
                    }
                }
                reader.Read();
            }
            reader.Read();
            return columnList;
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<string, dynamic> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}