using FluentValidation;
using Truestory.Common.Contracts;

namespace Truestory.Common.Validators;

public class ProductDTOValidator : AbstractValidator<ProductDTO>
{
    public ProductDTOValidator()
    {
        RuleFor(product => product.Name)
            .NotEmpty()
            .WithMessage("Product name is required.")
            .Length(1, 150)
            .WithMessage("Product name must be between 1 and 150 characters.");
    }
}
