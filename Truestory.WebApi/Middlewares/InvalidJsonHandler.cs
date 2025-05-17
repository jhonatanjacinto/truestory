using System;
using System.Text.Json;
using Truestory.WebApi.ApiResponses;

namespace Truestory.WebApi.Middlewares;

public class InvalidJsonHandler(RequestDelegate next, ILogger<InvalidJsonHandler> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.ContentType == "application/json")
        {
            try
            {
                context.Request.EnableBuffering();
                var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
                context.Request.Body.Position = 0;
                using var jsonDoc = JsonDocument.Parse(requestBody);
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Invalid JSON format in request.");
                await context.Response.WriteJsonApiErrorResponseAsync(
                    StatusCodes.Status400BadRequest,
                    "Invalid JSON format."
                );
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing the request.");
                await context.Response.WriteJsonApiErrorResponseAsync(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while processing the request: " + ex.Message
                );
                return;
            }
        }

        await next(context);
    }
}