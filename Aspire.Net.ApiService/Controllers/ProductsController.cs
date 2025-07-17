using Aspire.Net.ApiService.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aspire.Net.ApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService productService, ILogger<ProductsController> logger) : ControllerBase
{
    private readonly IProductService _productService = productService;
    private readonly ILogger<ProductsController> _logger = logger;

    /// <summary>
    /// Obtém todos os produtos ativos
    /// </summary>
    /// <returns>Lista de produtos</returns>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            _logger.LogInformation("Buscando produtos ativos paginados. Página {Page}, Tamanho {PageSize}", page, pageSize);

            var products = await _productService.GetAllProductsAsync(page, pageSize);

            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
        }
    }
}
