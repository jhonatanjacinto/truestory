using FluentValidation;
using Truestory.Common.Exceptions;

namespace Truestory.Common.Validators;

public static class FluentValidatorExtensions
{
    public static void ValidateWithTruestoryApiException<T>(this IValidator<T> validator, T instance)
    {
        var validationResult = validator.Validate(instance);
        if (!validationResult.IsValid)
        {
            var error = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed";
            throw new TruestoryApiException(error);
        }
    }
}
