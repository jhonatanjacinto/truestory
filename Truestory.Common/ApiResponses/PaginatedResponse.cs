namespace Truestory.Common.ApiResponses;

public record PaginatedResponse<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int TotalPages,
    int Page,
    int PageSize
) where T : class;
