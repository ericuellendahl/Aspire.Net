namespace Apire.Worker.Domain.DTOs;

public class PaymentResponseDto
{
    public int? Id { get; set; }
    public int? ProductId { get; set; }
    public int? TypePayment { get; set; }
    public decimal? Amount { get; set; }
    public string? Details { get; set; } = string.Empty;
    public DateTime? AtCreation { get; set; }
    public string? Status { get; set; } = "Pendente";
}
