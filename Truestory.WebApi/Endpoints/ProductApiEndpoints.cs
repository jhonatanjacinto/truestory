using System;
using Microsoft.EntityFrameworkCore;
using Truestory.WebApi.Database;

namespace Truestory.WebApi.Endpoints;

public static class ProductApiEndpoints
{
    public static WebApplication MapProductApiEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/products");

        // GET /products - Get all products
        group.MapGet("/", async (TruestoryDbContext context) =>
        {
            var products = await context.Products.ToListAsync();
            return Results.Ok(products);
        })
        .WithName("GetAllProducts");

        return app;
    }
}
