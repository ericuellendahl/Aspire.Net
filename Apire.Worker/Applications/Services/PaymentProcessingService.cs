using Apire.Worker.Domain.Entities;
using Apire.Worker.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Apire.Worker.Applications.Services
{
    public class PaymentProcessingService : IPaymentProcessingService
    {
        private readonly ILogger<PaymentProcessingService> _logger;
        private readonly IPaymentService _pagamentoService;

        public PaymentProcessingService(
            ILogger<PaymentProcessingService> logger,
            IPaymentService pagamentoService)
        {
            _logger = logger;
            _pagamentoService = pagamentoService;
        }

        public async Task ProcessPaymentAsync(PaymentMessage payment)
        {
            try
            {
                _logger.LogInformation("Processando pagamento ID: {PaymentId}", payment.Id);

                // Processar o pagamento através do serviço
                var resultado = await _pagamentoService.UpdateAsync(payment);

                _logger.LogInformation("Pagamento processado com sucesso. ID: {PaymentId}", payment.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar pagamento ID: {PaymentId}", payment.Id);
                throw;
            }
        }
    }
}
