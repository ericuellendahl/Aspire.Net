using Aspire.Net.ApiService.Domain.DTOs;

namespace Aspire.Net.ApiService.Domain.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>?> GetAllProductsAsync(int page, int pageSize);
        Task<ProductDto?> GetProductByIdAsync(int id);
    }
}
