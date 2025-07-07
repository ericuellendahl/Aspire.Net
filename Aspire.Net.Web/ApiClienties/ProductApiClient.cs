using Aspire.Net.Web.DTOs.Responses;

namespace Aspire.Net.Web.ApiClienties
{
    public class ProductApiClient(HttpClient httpClient)
    {
        public async Task<ProductResponse[]> GetProductsAsync(int maxItems = 10, CancellationToken cancellationToken = default)
        {
            List<ProductResponse>? products = null;
            await foreach (var product in httpClient.GetFromJsonAsAsyncEnumerable<ProductResponse>("/api/Products", cancellationToken))
            {
                if (products?.Count >= maxItems)
                {
                    break;
                }
                if (product is not null)
                {
                    products ??= [];
                    products.Add(product);
                }
            }
            return products?.ToArray() ?? [];
        }
    }
}
