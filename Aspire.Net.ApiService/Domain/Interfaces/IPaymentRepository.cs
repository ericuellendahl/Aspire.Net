using Aspire.Net.ApiService.Domain.Entities;

namespace Aspire.Net.ApiService.Domain.Interfaces;

public interface IPaymentRepository
{
    Task<Payment> CreateAsync(Payment payment);
    Task<Payment?> GetByIdAsync(int id);
    Task<IEnumerable<Payment>> GetAllAsync();
    Task<bool> UpdateAsync(Payment payment);
    Task<bool> DeleteAsync(int id);
}
