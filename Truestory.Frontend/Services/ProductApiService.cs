using System.Net;
using System.Text;
using System.Text.Json;
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
        catch (Exception ex) when (ex is not TruestoryApiException)
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
        catch (Exception ex) when (ex is not TruestoryApiException)
        {
            logger.LogError(ex, "An error occurred while fetching paginated products from Truestory API.");
            throw new TruestoryApiException("An unexpected error occurred while fetching paginated products.");
        }
    }

    public async Task<ProductDTO> GetProductByIdAsync(string? id)
    {
        try
        {
            var client = httpClientFactory.CreateClient("TruestoryApiClient");
            var response = await client.GetAsync($"/products/{id}");

            if (response.IsSuccessStatusCode)
            {
                var product = await response.Content.ReadFromJsonAsync<ProductDTO>();
                return product ?? throw new TruestoryApiException("Failed to fetch product: No content returned.");
            }

            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            if (errorResponse is not null)
            {
                throw new TruestoryApiException(errorResponse.Message);
            }

            throw new TruestoryApiException($"Failed to fetch product from Truestory API: {response.ReasonPhrase}");
        }
        catch (Exception ex) when (ex is not TruestoryApiException)
        {
            logger.LogError(ex, "An error occurred while fetching the product from Truestory API.");
            throw new TruestoryApiException("An unexpected error occurred while fetching the product.");
        }
    }

    public async Task DeleteProductAsync(string id)
    {
        try
        {
            var client = httpClientFactory.CreateClient("TruestoryApiClient");
            var response = await client.DeleteAsync($"/products/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                if (errorResponse is not null)
                {
                    throw new TruestoryApiException(errorResponse.Message);
                }

                throw new TruestoryApiException($"Failed to delete product in Truestory API: {response.ReasonPhrase}");
            }
        }
        catch (Exception ex) when (ex is not TruestoryApiException)
        {
            logger.LogError(ex, "An error occurred while deleting the product from Truestory API.");
            throw new TruestoryApiException("An unexpected error occurred while deleting the product.");
        }
    }

    public async Task<ProductDTO> UpdateProductAsync(string id, UpdateProductDTO productUpdated)
    {
        try
        {
            var client = httpClientFactory.CreateClient("TruestoryApiClient");
            var content = new StringContent(JsonSerializer.Serialize(productUpdated), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/products/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                var updatedProduct = await response.Content.ReadFromJsonAsync<ProductDTO>();
                return updatedProduct ?? throw new TruestoryApiException("Failed to update product: No content returned.");
            }

            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            if (errorResponse is not null)
            {
                throw new TruestoryApiException(errorResponse.Message);
            }

            throw new TruestoryApiException($"Failed to update product in Truestory API: {response.ReasonPhrase}");
        }
        catch (Exception ex) when (ex is not TruestoryApiException)
        {
            logger.LogError(ex, "An error occurred while updating the product from Truestory API.");
            throw new TruestoryApiException("An unexpected error occurred while updating the product.");
        }
    }
    
    public async Task<ProductDTO> CreateProductAsync(CreateProductDTO product)
    {
        try
        {
            var client = httpClientFactory.CreateClient("TruestoryApiClient");
            var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/products", content);

            if (response.IsSuccessStatusCode)
            {
                var createdProduct = await response.Content.ReadFromJsonAsync<ProductDTO>();
                return createdProduct ?? throw new TruestoryApiException("Failed to create product: No content returned.");
            }

            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            if (errorResponse is not null)
            {
                throw new TruestoryApiException(errorResponse.Message);
            }

            throw new TruestoryApiException($"Failed to create product in Truestory API: {response.ReasonPhrase}");
        }
        catch (Exception ex) when (ex is not TruestoryApiException)
        {
            logger.LogError(ex, "An error occurred while creating the product in Truestory API.");
            throw new TruestoryApiException("An unexpected error occurred while creating the product.");
        }
    }
}
