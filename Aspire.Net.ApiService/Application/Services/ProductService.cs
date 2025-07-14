using Aspire.Net.ApiService.Domain.DTOs;
using Aspire.Net.ApiService.Domain.Entities;
using Aspire.Net.ApiService.Domain.Interfaces;
using Aspire.Net.ApiService.Infrastrutura.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Aspire.Net.ApiService.Application.Services
{
    public class ProductService(ApplicationDbContext context, ILogger<ProductService> logger, CacheService cacheService) : IProductService
    {

        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<ProductService> _logger = logger;
        private readonly CacheService _cacheService = cacheService;

        const string keyProducts = "products";

        public async Task<IEnumerable<ProductDto>?> GetAllProductsAsync()
        {
            _logger.LogInformation("Fetching all active products from the database.");

            var cachedProducts = await _cacheService.GetAsync<IEnumerable<ProductDto>>(keyProducts);
            if (cachedProducts != null)
            {
                return cachedProducts;
            }

            var products = await _context.Products
                                        .AsNoTracking()
                                        .Where(p => p.IsActive)
                                        .OrderBy(p => p.Name)
                                        .ToListAsync();

            await _cacheService.SetAsync(keyProducts, products.Select(MapToDto), TimeSpan.FromMinutes(10));

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
