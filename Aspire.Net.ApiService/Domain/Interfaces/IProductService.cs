using Aspire.Net.ApiService.Domain.DTOs;

namespace Aspire.Net.ApiService.Domain.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>?> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
    }
}
