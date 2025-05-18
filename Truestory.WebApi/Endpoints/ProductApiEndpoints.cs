using System.Net;
using Microsoft.EntityFrameworkCore;
using Truestory.WebApi.ApiValidations;
using Truestory.Common.ApiResponses;
using Truestory.WebApi.Database;
using Truestory.WebApi.Services;
using Truestory.Common.Exceptions;
using Truestory.Common.Contracts;
using Truestory.Common.Validators;
using Truestory.WebApi.Adapters;
using Microsoft.AspNetCore.Mvc;

namespace Truestory.WebApi.Endpoints;

public static class ProductApiEndpoints
{
    public static WebApplication MapProductApiEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/products");

        // GET /products - Get all products
        group.MapGet("/", async ([FromQuery(Name = "filter")] string? term, TruestoryDbContext context) =>
        {
            try
            {
                if (!string.IsNullOrEmpty(term))
                {
                    var productsFiltered = await context.Products
                        .Where(p => p.Name.Contains(term))
                        .Select(p => ProductAdapter.ToDto(p))
                        .AsNoTracking()
                        .ToListAsync();

                    return Results.Ok(productsFiltered ?? []);
                }

                var products = await context.Products
                    .Select(p => ProductAdapter.ToDto(p))
                    .AsNoTracking()
                    .ToListAsync();

                return Results.Ok(products ?? []);
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(
                    new ErrorResponse(
                        (int)HttpStatusCode.InternalServerError,
                        $"An error occurred while fetching products: {ex.Message}"
                    )
                );
            }
        })
        .WithName("GetAllProducts");

        // GET /products/page/{page}/{pageSize} - Get products by page
        group.MapGet("/page/{page:int}/{pageSize:int=10}", async (int page, int pageSize, [FromQuery(Name = "filter")] string? term, TruestoryDbContext context) =>
        {
            if (page < 1 || pageSize < 1)
            {
                return Results.BadRequest(
                    new ErrorResponse(
                        (int)HttpStatusCode.BadRequest,
                        "Page and page size must be greater than 0."
                    )
                );
            }

            try
            {
                if (!string.IsNullOrEmpty(term))
                {
                    var totalFilteredProducts = await context.Products
                        .Where(p => p.Name.Contains(term))
                        .CountAsync();
                    var totalFilteredPages = (int)Math.Ceiling((double)totalFilteredProducts / pageSize);
                    if (page > totalFilteredPages)
                    {
                        return Results.NotFound(
                            new ErrorResponse(
                                (int)HttpStatusCode.NotFound,
                                "No products found for the specified page."
                            )
                        );
                    }

                    var productsFiltered = await context.Products
                        .Where(p => p.Name.Contains(term))
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .Select(p => ProductAdapter.ToDto(p))
                        .AsNoTracking()
                        .ToListAsync();

                    return Results.Ok(
                        new PaginatedResponse<ProductDTO>(
                            productsFiltered ?? [],
                            totalFilteredProducts,
                            totalFilteredPages,
                            page,
                            pageSize
                        )
                    );
                }

                var totalProducts = await context.Products.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
                if (page > totalPages)
                {
                    return Results.NotFound(
                        new ErrorResponse(
                            (int)HttpStatusCode.NotFound,
                            "No products found for the specified page."
                        )
                    );
                }

                var products = await context.Products
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => ProductAdapter.ToDto(p))
                    .AsNoTracking()
                    .ToListAsync();

                return Results.Ok(
                    new PaginatedResponse<ProductDTO>(
                        products ?? [],
                        totalProducts,
                        totalPages,
                        page,
                        pageSize
                    )
                );
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(
                    new ErrorResponse(
                        (int)HttpStatusCode.InternalServerError,
                        $"An error occurred while fetching products: {ex.Message}"
                    )
                );
            }
        })
        .WithName("GetProductsByPage");

        // GET /products/{id} - Get a product by ID
        group.MapGet("/{id}", async (string id, TruestoryDbContext context) =>
        {
            var product = await context.Products.FindAsync(id);
            return product is not null ?
                    Results.Ok(product) :
                    Results.NotFound(
                        new ErrorResponse(
                            (int)HttpStatusCode.NotFound,
                            $"Product with ID '{id}' not found."
                        )
                    );
        })
        .WithName("GetProductById");

        // POST /products - Create a new product
        group.MapPost("/", async (CreateProductDTO createProductDto, ProductExternalApiService productExternalApiService, TruestoryDbContext context) =>
        {
            try
            {
                var productDto = await productExternalApiService.CreateProductAsync(createProductDto);
                var product = ProductAdapter.ToEntity(productDto);
                product.UpdatedAt = product.CreatedAt;
                context.Products.Add(product);
                await context.SaveChangesAsync();
                return Results.CreatedAtRoute("GetProductById", new { id = product.Id }, ProductAdapter.ToDto(product));
            }
            catch (TruestoryApiException ex)
            {
                return Results.InternalServerError(
                    new ErrorResponse(
                        (int)HttpStatusCode.InternalServerError,
                        ex.Message
                    )
                );
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(
                    new ErrorResponse(
                        (int)HttpStatusCode.InternalServerError,
                        $"An error occurred while creating the product: {ex.Message}"
                    )
                );
            }
        })
        .WithName("CreateProduct")
        .WithValidation(new ProductDTOValidator<CreateProductDTO>());

        // PUT /products/{id} - Update an existing product
        group.MapPut("/{id}", async (string id, UpdateProductDTO productToUpdateDto, ProductExternalApiService productExternalApiService, TruestoryDbContext context) =>
        {
            try
            {
                var productDtoUpdated = await productExternalApiService.UpdateProductAsync(id, productToUpdateDto);
                var product = ProductAdapter.ToEntity(productDtoUpdated);
                var existingProduct = await context.Products.FindAsync(id);
                if (existingProduct is null)
                {
                    return Results.NotFound(
                        new ErrorResponse(
                            (int)HttpStatusCode.NotFound,
                            $"Product with ID '{id}' not found."
                        )
                    );
                }
                product.CreatedAt = existingProduct.CreatedAt;
                context.Entry(existingProduct).CurrentValues.SetValues(product);
                await context.SaveChangesAsync();
                return Results.Ok(ProductAdapter.ToDto(product));
            }
            catch (TruestoryApiException ex)
            {
                return Results.InternalServerError(
                    new ErrorResponse(
                        (int)HttpStatusCode.InternalServerError,
                        ex.Message
                    )
                );
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(
                    new ErrorResponse(
                        (int)HttpStatusCode.InternalServerError,
                        $"An error occurred while updating the product: {ex.Message}"
                    )
                );
            }
        })
        .WithName("UpdateProduct")
        .WithValidation(new ProductDTOValidator<UpdateProductDTO>());

        // PATCH /products/{id} - Partially update a product
        group.MapPatch("/{id}", async (string id, PatchProductDTO productToUpdateDto, ProductExternalApiService productExternalApiService, TruestoryDbContext context) =>
        {
            try
            {
                var productDtoUpdated = await productExternalApiService.PartialUpdateProductAsync(id, productToUpdateDto);
                var product = await context.Products.FindAsync(id);
                if (product is null)
                {
                    return Results.NotFound(
                        new ErrorResponse(
                            (int)HttpStatusCode.NotFound,
                            $"Product with ID '{id}' not found."
                        )
                    );
                }
                ProductAdapter.ApplyToEntity(product, productDtoUpdated);
                await context.SaveChangesAsync();
                return Results.Ok(ProductAdapter.ToDto(product));
            }
            catch (TruestoryApiException ex)
            {
                return Results.InternalServerError(
                    new ErrorResponse(
                        (int)HttpStatusCode.InternalServerError,
                        ex.Message
                    )
                );
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(
                    new ErrorResponse(
                        (int)HttpStatusCode.InternalServerError,
                        $"An error occurred while partially updating the product: {ex.Message}"
                    )
                );
            }
        })
        .WithName("PartialUpdateProduct")
        .WithValidation(new ProductDTOValidator<PatchProductDTO>());

        // DELETE /products/{id} - Delete a product
        group.MapDelete("/{id}", async (string id, ProductExternalApiService productExternalApiService, TruestoryDbContext context) =>
        {
            try
            {
                await productExternalApiService.DeleteProductAsync(id);
                var product = await context.Products.FindAsync(id);
                if (product is null)
                {
                    return Results.NotFound(
                        new ErrorResponse(
                            (int)HttpStatusCode.NotFound,
                            $"Could not delete product with ID {id}. Product not found!"
                        )
                    );
                }

                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return Results.NoContent();
            }
            catch (TruestoryApiException ex)
            {
                return Results.InternalServerError(
                    new ErrorResponse(
                        (int)HttpStatusCode.InternalServerError,
                        ex.Message
                    )
                );
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(
                    new ErrorResponse(
                        (int)HttpStatusCode.InternalServerError,
                        $"An error occurred while deleting the product: {ex.Message}"
                    )
                );
            }
        })
        .WithName("DeleteProduct");

        return app;
    }
}
