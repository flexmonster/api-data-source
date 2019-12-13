using DataAPI.Models;
using DataAPI.Models.Select;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataAPI.JsonConverters
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
                    writer.WriteString(new ReadOnlySpan<char>(new char[] { 'f', 'i', 'e', 'l', 'd', 's' }), field.Field);
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
                            if (value.Hits[i][j].StringValue != null)
                            {
                                writer.WriteStringValue(value.Hits[i][j].StringValue);
                            }
                            else if (value.Hits[i][j].DateValue != null)
                            {
                                writer.WriteNumberValue(value.Hits[i][j].DateValue.Value.ToUnixTimestamp());
                            }
                            else
                            {
                                writer.WriteNumberValue(value.Hits[i][j].NumberValue.Value);
                            }
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
                        foreach (var val in memberval.Value)
                        {
                            writer.WriteStartObject();
                            writer.WriteNumber(val.Key, val.Value);
                            writer.WriteEndObject();
                        }
                    }
                    writer.WriteEndObject();
                    if (member.Keys != null)
                    {
                        writer.WritePropertyName(new ReadOnlySpan<char>(new char[] { 'k', 'e', 'y', 's' }));
                        writer.WriteStartObject();
                        foreach (var memberkey in member.Keys)
                        {
                            if (memberkey.Value.StringValue != null)
                            {
                                writer.WriteString(memberkey.Key, memberkey.Value.StringValue);
                            }
                            else if (memberkey.Value.DateValue.HasValue)
                            {
                                writer.WriteString(memberkey.Key, memberkey.Value.DateValue.Value.ToUnixTimestamp().ToString());
                            }
                            else
                            {
                                writer.WriteString(memberkey.Key, memberkey.Value.NumberValue.Value.ToString());
                            }
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