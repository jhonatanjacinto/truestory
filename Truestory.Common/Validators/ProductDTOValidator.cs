using FluentValidation;
using Truestory.Common.Contracts;

namespace Truestory.Common.Validators;

public class ProductDTOValidator<T> : AbstractValidator<T> where T : IProductDTO
{
    public ProductDTOValidator()
    {
        When(product => product is UpdateProductDTO or CreateProductDTO, () =>
        {
            RuleFor(product => product.Name)
                .NotEmpty()
                .WithMessage("Product name is required.")
                .Length(1, 150)
                .WithMessage("Product name must be between 1 and 150 characters.");
        });

        When(product => product is PatchProductDTO, () =>
        {
            // Validate the property name only if explicitly Name isn't an empty string
            RuleFor(product => product.Name)
                .NotEmpty()
                .WithMessage("Product name is required.")
                .When(product => product.Name == string.Empty)
                .Length(1, 150)
                .WithMessage("Product name must be between 1 and 150 characters.");
        });
    }
}
