namespace Truestory.WebApi.ApiResponses;

public record class ExternalApiError
{
    public string? Error { get; init; } = null!;
}
