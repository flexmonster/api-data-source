using NetCoreServer.Models.Select;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCoreServer.JsonConverters
{
    /// <summary>
    /// Convertor from SelectResponse object to JSON
    /// </summary>
    public class SelectResponseJsonConverter : JsonConverter<SelectResponse>
    {
        public override SelectResponse Read(ref Utf8JsonReader reader,
                                 Type typeToConvert,
                                 JsonSerializerOptions options)
        {
            var response = new SelectResponse();
            response = JsonSerializer.Deserialize<SelectResponse>(ref reader, options);
            return response;
        }

        public override void Write(Utf8JsonWriter writer,
                                   SelectResponse value,
                                   JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            if (value.Fields != null)
            {
                writer.WritePropertyName(new ReadOnlySpan<char>(new char[] { 'f', 'i', 'e', 'l', 'd', 's' }));

                writer.WriteStartArray();
                foreach (var field in value.Fields)
                {
                    writer.WriteStartObject();
                    writer.WriteString(new ReadOnlySpan<char>(new char[] { 'u', 'n', 'i', 'q', 'u', 'e', 'N', 'a', 'm', 'e' }), field.UniqueName);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }
            if (value.Hits != null)
            {
                writer.WritePropertyName(new ReadOnlySpan<char>(new char[] { 'h', 'i', 't', 's' }));
                writer.WriteStartArray();
                for (int i = 0; i < value.Hits.Count; i++)
                {
                    writer.WriteStartArray();
                    for (int j = 0; j < value.Hits[i].Count; j++)
                    {
                        if (value.Hits[i][j] != null)
                        {
                            JsonSerializer.Serialize(writer, value.Hits[i][j], options);
                        }
                        else
                        {
                            writer.WriteNullValue();
                        }
                    }
                    writer.WriteEndArray();
                }
                writer.WriteEndArray();
            }
            if (value.Aggs != null)
            {
                writer.WritePropertyName(new ReadOnlySpan<char>(new char[] { 'a', 'g', 'g', 's' }));
                writer.WriteStartArray();
                foreach (var member in value.Aggs)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName(new ReadOnlySpan<char>(new char[] { 'v', 'a', 'l', 'u', 'e', 's' }));
                    writer.WriteStartObject();
                    foreach (var memberval in member.Values)
                    {
                        writer.WritePropertyName(memberval.Key);
                        writer.WriteStartObject();
                        foreach (var val in memberval.Value)
                        {
                            writer.WriteNumber(val.Key, val.Value);
                        }
                        writer.WriteEndObject();
                    }
                    writer.WriteEndObject();
                    if (member.Keys != null)
                    {
                        writer.WritePropertyName(new ReadOnlySpan<char>(new char[] { 'k', 'e', 'y', 's' }));
                        writer.WriteStartObject();
                        foreach (var memberkey in member.Keys)
                        {
                            writer.WritePropertyName(memberkey.Key);
                            JsonSerializer.Serialize(writer, memberkey.Value, options);
                        }
                        writer.WriteEndObject();
                    }
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }
            writer.WriteNumber(new ReadOnlySpan<char>(new char[] { 'p', 'a', 'g', 'e' }), value.Page);
            writer.WriteNumber(new ReadOnlySpan<char>(new char[] { 'p', 'a', 'g', 'e', 'T', 'o', 't', 'a', 'l' }), value.PageTotal);

            writer.WriteEndObject();
        }
    }
}