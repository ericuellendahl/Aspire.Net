using Aspire.Net.ApiService.Domain.DTOs;

namespace Aspire.Net.ApiService.Domain.Interfaces;

public interface IPaymentService
{
    Task<PaymentResponseDto> CreateAsync(PaymentRequestDto paymentRequestDto);
    Task<PaymentResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<PaymentResponseDto>> GetAllAsync();
}
