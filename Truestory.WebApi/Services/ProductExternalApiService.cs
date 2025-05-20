using System.Text.Json;
using Truestory.Common.Contracts;
using Truestory.WebApi.ApiResponses;
using Truestory.WebApi.Exceptions;

namespace Truestory.WebApi.Services;

/**
* This class is responsible for interacting with external Product APIs.
* It handles the logic for making HTTP requests to the external API from where all the data will be fetched.
*/
public class ProductExternalApiService(
    IHttpClientFactory httpClientFactory,
    ILogger<ProductExternalApiService> logger)
{
    // Get all products from the external API
    public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
    {
        try
        {
            var client = httpClientFactory.CreateClient("ExternalApiClient");
            var response = await client.GetAsync("/objects");

            if (response.IsSuccessStatusCode)
            {
                var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDTO>>();
                return products ?? [];
            }

            throw new ExternalApiServiceException($"Failed to fetch products from external API: {response.ReasonPhrase}");
        }
        catch (ExternalApiServiceException)
        {
            logger.LogError("External API service exception occurred.");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching products from external API.");
            throw new ExternalApiServiceException($"Failed to fetch products from external API.");
        }
    }

    // Delete a product by ID from the external API
    public async Task DeleteProductAsync(string id)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ExternalApiClient");
            var response = await client.DeleteAsync($"/objects/{id}");

            if (!response.IsSuccessStatusCode)
            {
                throw new ExternalApiServiceException($"Failed to delete product with ID '{id}' from external API: {response.ReasonPhrase}");
            }
        }
        catch (ExternalApiServiceException)
        {
            logger.LogError("External API service exception occurred.");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while deleting product from external API.");
            throw new ExternalApiServiceException($"Failed to delete product from external API.");
        }
    }

    // Create a new product in the external API
    public async Task<ProductDTO> CreateProductAsync(CreateProductDTO productDto)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ExternalApiClient");
            var response = await client.PostAsJsonAsync("/objects", productDto);
            if (response.IsSuccessStatusCode)
            {
                var createdProduct = await response.Content.ReadFromJsonAsync<ProductDTO>();
                return createdProduct ?? throw new ExternalApiServiceException($"Failed to create product in external API.");
            }

            var responseInfo = await response.Content.ReadFromJsonAsync<ExternalApiError>();
            if (responseInfo is not null)
            {
                var errorMessage = responseInfo.Error ?? "Unknown error";
                throw new ExternalApiServiceException($"Failed to create product in external API: {errorMessage}");
            }

            throw new ExternalApiServiceException($"Failed to create product in external API: {response.ReasonPhrase}");
        }
        catch (ExternalApiServiceException)
        {
            logger.LogError("External API service exception occurred.");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating product in external API.");
            throw new ExternalApiServiceException($"Failed to create product in external API.");
        }
    }

    // Update an existing product in the external API
    public async Task<ProductDTO> UpdateProductAsync(string id, UpdateProductDTO productDto)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ExternalApiClient");
            var response = await client.PutAsJsonAsync($"/objects/{id}", productDto);
            if (response.IsSuccessStatusCode)
            {
                var updatedProduct = await response.Content.ReadFromJsonAsync<ProductDTO>();
                return updatedProduct ?? throw new ExternalApiServiceException($"Failed to update product with ID '{id}' in external API.");
            }

            var responseInfo = await response.Content.ReadFromJsonAsync<ExternalApiError>();
            if (responseInfo is not null)
            {
                var errorMessage = responseInfo.Error ?? "Unknown error";
                throw new ExternalApiServiceException($"Failed to update product in external API: {errorMessage}");
            }

            throw new ExternalApiServiceException($"Failed to update product in external API: {response.ReasonPhrase}");
        }
        catch (ExternalApiServiceException)
        {
            logger.LogError("External API service exception occurred.");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while updating product in external API.");
            throw new ExternalApiServiceException($"Failed to update product in external API.");
        }
    }
    
    public async Task<ProductDTO> PartialUpdateProductAsync(string id, PatchProductDTO productDto)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ExternalApiClient");
            var response = await client.PatchAsJsonAsync($"/objects/{id}", productDto);
            if (response.IsSuccessStatusCode)
            {
                var updatedProduct = await response.Content.ReadFromJsonAsync<ProductDTO>();
                return updatedProduct ?? throw new ExternalApiServiceException($"Failed to update product with ID '{id}' in external API.");
            }

            var responseInfo = await response.Content.ReadFromJsonAsync<ExternalApiError>();
            if (responseInfo is not null)
            {
                var errorMessage = responseInfo.Error ?? "Unknown error";
                throw new ExternalApiServiceException($"Failed to update product in external API: {errorMessage}");
            }

            throw new ExternalApiServiceException($"Failed to update product in external API: {response.ReasonPhrase}");
        }
        catch (ExternalApiServiceException)
        {
            logger.LogError("External API service exception occurred.");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while updating product in external API.");
            throw new ExternalApiServiceException($"Failed to update product in external API.");
        }
    }
}
