using DataAPI.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataAPI.JsonConverters
{
    /// <summary>
    /// Convertor from Value object to JSON depending on it's type
    /// </summary>
    public class ValuesJsonConverter : JsonConverter<Value>
    {
        public override Value Read(ref Utf8JsonReader reader,
                                 Type typeToConvert,
                                 JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (DateTime.TryParse(reader.GetString(), out DateTime date))
                {
                    return new Value(date);
                }
                else
                {
                    var value = reader.GetString();
                    return new Value(value);
                }                
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {

                var intvalue = reader.GetDouble();
                return new Value(intvalue);
            }
            reader.Read();
            if (reader.TokenType == JsonTokenType.String)
            {
                var list = new List<string>();
                list.Add(reader.GetString());
                reader.Read();
                list.Add(reader.GetString());
                reader.Read();
                return new Value(list);
            }
            var numberList = new List<double>();
            numberList.Add(reader.GetDouble());
            reader.Read();
            numberList.Add(reader.GetDouble());
            reader.Read();
            return new Value(numberList);
        }

        public override void Write(Utf8JsonWriter writer,
                                   Value value,
                                   JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value);
        }
    }

}
