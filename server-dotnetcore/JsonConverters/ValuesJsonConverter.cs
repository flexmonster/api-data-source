using NetCoreServer.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCoreServer.JsonConverters
{
    /// <summary>
    /// Convertor from ValueObject object to JSON depending on it's type
    /// </summary>
    public class ValuesJsonConverter : JsonConverter<ValueObject>
    {
        public override ValueObject Read(ref Utf8JsonReader reader,
                                 Type typeToConvert,
                                 JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var value = reader.GetString();
                return new ValueObject(value);
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                var intvalue = reader.GetDouble();
                return new ValueObject(intvalue);
            }
            reader.Read();
            if (reader.TokenType == JsonTokenType.String)
            {
                var list = new List<string>();
                list.Add(reader.GetString());
                reader.Read();
                list.Add(reader.GetString());
                reader.Read();
                return new ValueObject(list);
            }
            var numberList = new List<double>();
            numberList.Add(reader.GetDouble());
            reader.Read();
            numberList.Add(reader.GetDouble());
            reader.Read();
            return new ValueObject(numberList);
        }

        public override void Write(Utf8JsonWriter writer,
                                   ValueObject value,
                                   JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value);
        }
    }
}