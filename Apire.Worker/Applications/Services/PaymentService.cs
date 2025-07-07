using Apire.Worker.Domain.Entities;
using Apire.Worker.Domain.Interfaces;
using Apire.Worker.Infraestrutura.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Apire.Worker.Applications.Services
{
    internal class PaymentService(ApplicationDbContext applicationDbContext, ILogger<PaymentService> logger) : IPaymentService
    {

        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
        private readonly ILogger<PaymentService> _logger = logger;

        public async Task<bool> UpdateAsync(PaymentMessage paymentMessage)
        {

            _logger.LogInformation("Atualizando pagamento ID: {PaymentId}", paymentMessage.Id);

            try
            {
                var existingPayment = await _applicationDbContext.Payments.FindAsync(paymentMessage.Id);

                if (existingPayment == null)
                {
                    _logger.LogWarning("Pagamento ID: {PaymentId} não encontrado.", paymentMessage.Id);
                    return false;
                }
                existingPayment.Status = "Confirmed";

                _applicationDbContext.Payments.Update(existingPayment);

                await _applicationDbContext.SaveChangesAsync();

                _logger.LogInformation("Pagamento ID: {PaymentId} atualizado com sucesso.", paymentMessage.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar pagamento ID: {PaymentId}", paymentMessage.Id);
                return false;
            }
        }
    }
}
