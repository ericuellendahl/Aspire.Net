using Int.Database.Domain.DTOs;

namespace Int.Database.Domain.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
    }
}
