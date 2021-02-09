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
            writer.WritePropertyName("members");
            writer.WriteStartArray();
            foreach (var member in value.Members)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("value");
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
            writer.WriteBoolean("sorted", value.Sorted);
            writer.WriteNumber("page", value.Page);
            writer.WriteNumber("pageTotal", value.PageTotal);

            writer.WriteEndObject();
        }
    }
}