using System.Net;
using Truestory.WebApi.ApiResponses;

namespace Truestory.WebApi.Endpoints;

public static class FallbackEndpoints
{
    public static void MapFallbackEndpoints(this WebApplication app)
    {
        app.MapFallback(() => Results.NotFound(
            new ErrorResponse(
                (int)HttpStatusCode.NotFound,
                "Endpoint mapping not found"
            )
        ));
    }
}
