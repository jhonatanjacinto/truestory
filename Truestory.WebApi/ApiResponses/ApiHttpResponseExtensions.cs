using System;

namespace Truestory.WebApi.ApiResponses;

public static class ApiHttpResponseExtensions
{
    public static Task WriteJsonApiErrorResponseAsync(this HttpResponse response, int statusCode, string message)
    {
        response.ContentType = "application/json";
        response.StatusCode = statusCode;
        return response.WriteAsJsonAsync(new ErrorResponse(statusCode, message));
    }
}
