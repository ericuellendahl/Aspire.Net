using Aspire.Net.ApiService.Domain.DTOs;
using Aspire.Net.ApiService.Domain.Entities;
using Aspire.Net.ApiService.Domain.Interfaces;
using Aspire.Net.ApiService.Infrastrutura.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Aspire.Net.ApiService.Application.Services
{
    public class ProductService(ApplicationDbContext _context, ILogger<ProductService> _logger, CacheService _cacheService) : IProductService
    {
        public async Task<IEnumerable<ProductDto>?> GetAllProductsAsync(int page, int pageSize)
        {
            _logger.LogInformation("Fetching all active products from the database.");

            string keyProducts = $"Idempotent_{page}{pageSize}";

            var cachedProducts = await _cacheService.GetAsync<IEnumerable<ProductDto>>(keyProducts);
            if (cachedProducts != null)
            {
                return cachedProducts;
            }

            var products = await _context.Products
                                        .AsNoTracking()
                                        .Where(p => p.IsActive)
                                        .OrderBy(p => p.Id)
                                        .Skip((page - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();

            await _cacheService.SetAsync(keyProducts, products.Select(MapToDto), TimeSpan.FromMinutes(2));

            return products.Select(MapToDto);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return product != null ? MapToDto(product) : null;
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                IsActive = product.IsActive,
                Category = product.Category,
                Sku = product.Sku,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }
    }
}
