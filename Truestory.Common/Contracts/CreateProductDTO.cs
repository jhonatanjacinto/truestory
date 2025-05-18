using System.Text.Json.Serialization;

namespace Truestory.Common.Contracts;

public record class CreateProductDTO : IProductDTO
{
    [JsonPropertyName("name")]
    public string? Name { get; init; } = string.Empty;

    [JsonPropertyName("data")]
    public Dictionary<string, dynamic>? Data { get; init; }
}
