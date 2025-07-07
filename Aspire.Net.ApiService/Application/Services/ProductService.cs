using Aspire.Net.ApiService.Domain.DTOs;
using Aspire.Net.ApiService.Domain.Entities;
using Aspire.Net.ApiService.Domain.Interfaces;
using Aspire.Net.ApiService.Infrastrutura.Brokers;
using Aspire.Net.ApiService.Infrastrutura.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Aspire.Net.ApiService.Application.Services
{
    public class ProductService(ApplicationDbContext context, ILogger<ProductService> logger, PagamentoProducerMQ pagamentoProducer) : IProductService
    {

        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<ProductService> _logger = logger;
        private readonly PagamentoProducerMQ _pagamentoProducer = pagamentoProducer;

        public async Task<IEnumerable<ProductDto>?> GetAllProductsAsync()
        {
            _logger.LogInformation("Fetching all active products from the database.");

            var products = await _context.Products
                                        .AsNoTracking()
                                        .Where(p => p.IsActive)
                                        .OrderBy(p => p.Name)
                                        .ToListAsync();

            var productDto = products.Select(MapToDto);

            if (productDto.Any())
            {
                var json = JsonSerializer.Serialize(productDto);
                _pagamentoProducer.EnviarMensagem(json);
            }

            return productDto;
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
