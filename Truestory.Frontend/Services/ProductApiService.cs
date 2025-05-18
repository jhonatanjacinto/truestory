using System;
using Truestory.Common.ApiResponses;
using Truestory.Common.Contracts;
using Truestory.Common.Exceptions;

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

            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            if (errorResponse is not null)
            {
                throw new TruestoryApiException(errorResponse.Message);
            }

            throw new TruestoryApiException($"Failed to fetch products from Truestory API: {response.ReasonPhrase}");
        }
        catch (TruestoryApiException ex)
        {
            logger.LogError(ex, "Truestory API exception occurred.");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching products from Truestory API.");
            throw;
        }
    }

    public async Task<PaginatedResponse<ProductDTO>?> GetPaginatedProductsAsync(int page, int pageSize = 10)
    {
        try
        {
            var client = httpClientFactory.CreateClient("TruestoryApiClient");
            var response = await client.GetAsync($"/products/page/{page}/{pageSize}");

            if (response.IsSuccessStatusCode)
            {
                var paginatedResponse = await response.Content.ReadFromJsonAsync<PaginatedResponse<ProductDTO>>();
                return paginatedResponse;
            }

            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            if (errorResponse is not null)
            {
                throw new TruestoryApiException(errorResponse.Message);
            }

            throw new TruestoryApiException($"Failed to fetch paginated products from Truestory API: {response.ReasonPhrase}");
        }
        catch (TruestoryApiException ex)
        {
            logger.LogError(ex, "Truestory API exception occurred.");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching paginated products from Truestory API.");
            throw;
        }
    }
}
