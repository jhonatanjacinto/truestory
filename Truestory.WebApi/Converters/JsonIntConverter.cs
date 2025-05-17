using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Truestory.WebApi.Converters;

public class JsonIntConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            return reader.TokenType switch
            {
                JsonTokenType.Number => reader.GetInt32(),
                JsonTokenType.String => int.TryParse(reader.GetString(), out var result) ? result : 0,
                _ => 0
            };
        }
        catch
        {
            return 0;
        }
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}
