using Aspire.Net.ApiService.Domain.DTOs;
using Aspire.Net.ApiService.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aspire.Net.ApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController(ILogger<PaymentController> logger, IPaymentService paymentService) : ControllerBase
{
    private readonly ILogger<PaymentController> _logger = logger;
    private readonly IPaymentService _paymentService = paymentService;


    [HttpPost]
    [Authorize]
    public async Task<ActionResult<PaymentResponseDto>> CriarPagamento([FromBody] PaymentRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pagamento = await _paymentService.CreateAsync(dto);
            return CreatedAtAction(nameof(ObterPagamento), new { id = pagamento.Id }, pagamento);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pagamento");
            return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<PaymentResponseDto>> ObterPagamento(int id)
    {
        try
        {
            var pagamento = await _paymentService.GetByIdAsync(id);
            return pagamento != null ? Ok(pagamento) : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pagamento");
            return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
        }
    }
}
