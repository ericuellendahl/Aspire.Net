namespace Aspire.Net.ApiService.Domain.DTOs
{
    public class PaymentRequestDto
    {
        public int ProductId { get; set; }
        public int TypePayment { get; set; }
        public decimal Amount { get; set; }
        public string Details { get; set; } = string.Empty;
    }
}
