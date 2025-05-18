using System.Text.Json.Serialization;

namespace Truestory.Common.Contracts;

public record class PatchProductDTO : IProductDTO
{
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; init; }

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, dynamic>? Data { get; init; }
}
