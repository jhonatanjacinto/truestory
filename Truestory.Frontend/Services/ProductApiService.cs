using System;
using Truestory.Common.Contracts;

namespace Truestory.Frontend.Services;

public class ProductApiService(IHttpClientFactory httpClientFactory, ILogger<ProductApiService> logger)
{
    public async Task<List<ProductDTO>> GetAllProductsAsync()
    {
        try
        {
            var client = httpClientFactory.CreateClient("TruestoryApiClient");
            var response = await client.GetAsync("/products");

            if (response.IsSuccessStatusCode)
            {
                var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDTO>>();
                return products is not null ? [.. products] : [];
            }

            throw new Exception($"Failed to fetch products from Truestory API: {response.ReasonPhrase}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching products from Truestory API.");
            throw;
        }
    }

    public async Task<List<ProductDTO>> GetPaginatedProductsAsync(int page, int pageSize = 10)
    {
        try
        {
            var client = httpClientFactory.CreateClient("TruestoryApiClient");
            var response = await client.GetAsync($"/products/page/{page}/{pageSize}");

            if (response.IsSuccessStatusCode)
            {
                var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDTO>>();
                return products is not null ? [.. products] : [];
            }

            throw new Exception($"Failed to fetch paginated products from Truestory API: {response.ReasonPhrase}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching paginated products from Truestory API.");
            throw;
        }
    }
}
