using Aspire.Net.Web.DTOs.Responses;
using System.Net.Http.Headers;

namespace Aspire.Net.Web.ApiEndPoints
{
    public class ProductApiClientService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<List<ProductResponse>> GetProductsPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["access_token"];
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = $"products/api/Products?page={page}&pageSize={pageSize}";
            var products = await httpClient.GetFromJsonAsync<List<ProductResponse>>(url, cancellationToken);

            return products ?? [];
        }
    }
}
