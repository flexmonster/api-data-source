using NetCoreServer.Models.DataModels;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCoreServer.JsonConverters
{
    /// <summary>
    /// Converter from JSON to ColumnType
    /// </summary>
    public class ColumnTypeJsonConverter : JsonConverter<ColumnType>
    {

        public override ColumnType Read(ref Utf8JsonReader reader,
                                 Type typeToConvert,
                                 JsonSerializerOptions options)
        {
            var type = JsonSerializer.Deserialize<string>(ref reader, options);
            if (type == "number")
            {
                return ColumnType.doubleType;
            }
            else if (type == "date")
            {
                return ColumnType.dateType;
            }
            return ColumnType.stringType;
        }

        public override void Write(Utf8JsonWriter writer,
                                   ColumnType value,
                                   JsonSerializerOptions options)
        {
            if (value == ColumnType.stringType)
                writer.WriteStringValue("string");
            else if (value == ColumnType.doubleType)
                writer.WriteStringValue("number");
            else if (value == ColumnType.dateType)
                writer.WriteStringValue("date");
        }
    }
}