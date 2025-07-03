using Int.Database.Domain.DTOs;
using Int.Database.Domain.Entities;
using Int.Database.Domain.Interfaces;
using Int.Database.Infrastrutura.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Int.Database.Application.Services
{
    public class ProductService(ApplicationDbContext context, ILogger<ProductService> logger) : IProductService
    {

        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<ProductService> _logger = logger;

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            _logger.LogInformation("Fetching all active products from the database.");

            var products = await _context.Products
                                        .AsNoTracking()
                                        .Where(p => p.IsActive)
                                        .OrderBy(p => p.Name)
                                        .ToListAsync();

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
