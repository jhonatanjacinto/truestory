using System.Text.Json.Serialization;

namespace Truestory.Common.Contracts;

public record class ProductDTO : IProductDTO
{
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string? Name { get; init; } = string.Empty;

    [JsonPropertyName("data")]
    public Dictionary<string, dynamic>? Data { get; init; }

    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; init; }

    [JsonPropertyName("updatedAt")]
    public DateTime? UpdatedAt { get; init; }
}
