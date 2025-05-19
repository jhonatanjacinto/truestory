using Truestory.Common.Contracts;
using Truestory.WebApi.Entities;

namespace Truestory.WebApi.Adapters;

public static class ProductAdapter
{
    public static ProductDTO ToDto(Product product)
    {
        return new ProductDTO
        {
            Id = product.Id,
            Name = product.Name,
            Data = product.Data,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    public static Product ToEntity(ProductDTO productDto)
    {
        return new Product
        {
            Id = productDto.Id,
            Name = productDto.Name ?? string.Empty,
            Data = productDto.Data,
            CreatedAt = productDto.CreatedAt,
            UpdatedAt = productDto.UpdatedAt
        };
    }

    public static void ApplyToEntity(Product product, ProductDTO productDto)
    {
        product.Name = string.IsNullOrEmpty(productDto.Name) ? product.Name : productDto.Name;

        if (productDto.Data is not null && productDto.Data.Count > 0)
        {
            product.Data ??= [];

            foreach (var key in productDto.Data.Keys)
            {
                if (product.Data.ContainsKey(key))
                {
                    product.Data[key] = productDto.Data[key];
                }
                else
                {
                    product.Data.Add(key, productDto.Data[key]);
                }
            }
        }

        product.UpdatedAt = productDto.UpdatedAt ?? product.UpdatedAt;
        product.CreatedAt = productDto.CreatedAt ?? product.CreatedAt;
    }
}
