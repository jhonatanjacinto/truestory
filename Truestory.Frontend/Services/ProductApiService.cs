using System.Net;
using Microsoft.AspNetCore.WebUtilities;
using Truestory.Common.ApiResponses;
using Truestory.Common.Contracts;
using Truestory.Common.Exceptions;

namespace Truestory.Frontend.Services;

public class ProductApiService(IHttpClientFactory httpClientFactory, ILogger<ProductApiService> logger)
{
    public async Task<List<ProductDTO>> GetAllProductsAsync(string? term = null)
    {
        try
        {
            var client = httpClientFactory.CreateClient("TruestoryApiClient");
            var queryParams = new Dictionary<string, string?>();
            
            if (!string.IsNullOrEmpty(term))
            {
                queryParams.Add("filter", term);
            }

            var url = QueryHelpers.AddQueryString("/products", queryParams);
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDTO>>();
                return products is not null ? [.. products] : [];
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return [];
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
            throw new TruestoryApiException("An unexpected error occurred while fetching products.");
        }
    }

    public async Task<PaginatedResponse<ProductDTO>?> GetPaginatedProductsAsync(int page, int pageSize = 10, string? term = null)
    {
        try
        {
            var client = httpClientFactory.CreateClient("TruestoryApiClient");
            var queryParams = new Dictionary<string, string?>();
            
            if (!string.IsNullOrEmpty(term))
            {
                queryParams.Add("filter", term);
            }

            var url = QueryHelpers.AddQueryString($"/products/page/{page}/{pageSize}", queryParams);
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var paginatedResponse = await response.Content.ReadFromJsonAsync<PaginatedResponse<ProductDTO>>();
                return paginatedResponse;
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
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
            throw new TruestoryApiException("An unexpected error occurred while fetching paginated products.");
        }
    }
}
