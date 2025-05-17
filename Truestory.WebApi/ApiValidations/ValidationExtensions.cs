using System;
using System.Net;
using System.Text.Json;
using FluentValidation;
using Truestory.WebApi.ApiResponses;

namespace Truestory.WebApi.ApiValidations;

public static class ValidationExtensions
{
    public static void WithValidation<T>(this IEndpointConventionBuilder builder, IValidator<T> validator)
    {
        builder.Add(b =>
        {
            var originalRequestDelegate = b.RequestDelegate;
            b.RequestDelegate = async context =>
            {
                var request = context.Request;

                try
                {
                    request.EnableBuffering();
                    var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
                    request.Body.Position = 0;
                    var requestObject = JsonSerializer.Deserialize<T>(requestBody) ?? Activator.CreateInstance<T>();
                    var validationResult = await validator.ValidateAsync(requestObject);

                    if (!validationResult.IsValid)
                    {
                        throw new ValidationException(
                            validationResult.Errors.FirstOrDefault()?.ErrorMessage
                            ?? "Validation failed for " + typeof(T).Name
                        );
                    }
                }
                catch (ValidationException ex)
                {
                    await context.Response.WriteJsonApiErrorResponseAsync(
                        StatusCodes.Status400BadRequest,
                        ex.Message
                    );
                    return;
                }
                catch (JsonException)
                {
                    await context.Response.WriteJsonApiErrorResponseAsync(
                        StatusCodes.Status400BadRequest,
                        "Invalid JSON format."
                    );
                    return;
                }
                catch (Exception ex)
                {
                    await context.Response.WriteJsonApiErrorResponseAsync(
                        StatusCodes.Status500InternalServerError,
                        "An error occurred while processing the request: " + ex.Message
                    );
                    return;
                }
                finally
                {
                    if (originalRequestDelegate != null)
                    {
                        await originalRequestDelegate(context);
                    }
                }
            };
        });
    }
}
