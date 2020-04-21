using NetCoreServer.Extensions;
using NetCoreServer.Models.Members;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCoreServer.JsonConverters
{
    /// <summary>
    /// Convertor from MembersResponse object to JSON
    /// </summary>
    public class MembersResponseJsonConverter : JsonConverter<MembersResponse>
    {
        public override MembersResponse Read(ref Utf8JsonReader reader,
                                 Type typeToConvert,
                                 JsonSerializerOptions options)
        {
            var response = new MembersResponse();
            response = JsonSerializer.Deserialize<MembersResponse>(ref reader, options);
            return response;
        }

        public override void Write(Utf8JsonWriter writer,
                                   MembersResponse value,
                                   JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(new ReadOnlySpan<char>(new char[] { 'm', 'e', 'm', 'b', 'e', 'r', 's' }));
            writer.WriteStartArray();
            foreach (var member in value.Members)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(new ReadOnlySpan<char>(new char[] { 'v', 'a', 'l', 'u', 'e' }));
                if (member != null)
                {
                    JsonSerializer.Serialize(writer, member, options);
                }
                else
                {
                    writer.WriteStringValue("");
                }
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            writer.WriteBoolean(new ReadOnlySpan<char>(new char[] { 's', 'o', 'r', 't', 'e', 'd' }), value.Sorted);
            writer.WriteNumber(new ReadOnlySpan<char>(new char[] { 'p', 'a', 'g', 'e' }), value.Page);
            writer.WriteNumber(new ReadOnlySpan<char>(new char[] { 'p', 'a', 'g', 'e', 'T', 'o', 't', 'a', 'l' }), value.PageTotal);

            writer.WriteEndObject();
        }
    }
}