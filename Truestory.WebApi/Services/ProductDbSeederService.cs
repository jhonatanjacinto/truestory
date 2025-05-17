using System;
using Microsoft.EntityFrameworkCore;
using Truestory.WebApi.Database;
using Truestory.WebApi.Entities;

namespace Truestory.WebApi.Services;

/**
* ProductDbSeederService is a hosted service that seeds the database with product data
* from an external API when the application starts.
* This avoids the need to reaching the external API every time a product is requested.
*/
public class ProductDbSeederService(
    IServiceProvider serviceProvider,
    ILogger<ProductDbSeederService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting ProductDbSeederService...");
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TruestoryDbContext>();
        var productExternalService = scope.ServiceProvider.GetRequiredService<ProductExternalApiService>();

        try
        {
            logger.LogInformation("ProductDbSeederService is running...");

            // Check if the database is empty
            if (!await dbContext.Products.AnyAsync(cancellationToken))
            {
                logger.LogInformation("Database is empty. Seeding data...");

                // Fetch products from the external API
                var productDtos = await productExternalService.GetAllProductsAsync();
                if (productDtos == null || !productDtos.Any())
                {
                    logger.LogWarning("No products found in the external API.");
                    return;
                }

                // Map DTOs to entities
                var products = productDtos.Select(p => new Product
                {
                    Id = p.Id,
                    Name = p.Name,
                    Data = p.Data,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                }).ToList();

                // Add products to the database
                await dbContext.Products.AddRangeAsync(products, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                logger.LogInformation("Database seeded with {Count} products.", products.Count);
            }
            else
            {
                logger.LogInformation("Database already contains data. Skipping seeding.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError("An error occurred while seeding the database: {Error}", ex.Message);
        }
        finally
        {
            logger.LogInformation("ProductDbSeederService has completed.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
