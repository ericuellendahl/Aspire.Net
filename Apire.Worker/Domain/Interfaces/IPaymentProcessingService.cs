using Apire.Worker.Domain.Entities;

namespace Apire.Worker.Domain.Interfaces;

public interface IPaymentProcessingService
{
    Task ProcessPaymentAsync(PaymentMessage payment);
}
