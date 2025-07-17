using Aspire.Net.Web.DTOs.Responses;
using System.Net.Http.Headers;

namespace Aspire.Net.Web.ApiEndPoints
{
    public class ProductApiClientService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<ProductResponse[]> GetProductsAsync(int maxItems = 10, CancellationToken cancellationToken = default)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["access_token"];
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            List<ProductResponse>? products = null;
            await foreach (var product in httpClient.GetFromJsonAsAsyncEnumerable<ProductResponse>("products/api/Products", cancellationToken))
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
