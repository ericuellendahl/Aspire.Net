using Apire.Worker.Domain.Entities;

namespace Apire.Worker.Domain.Interfaces;

public interface IPaymentService
{
    Task<bool> UpdateAsync(PaymentMessage paymentMessage);
}
