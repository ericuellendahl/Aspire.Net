namespace Apire.Worker.Domain.Entities;

public class PaymentMessage
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int TypePayment { get; set; }
    public decimal Amount { get; set; }
    public string Details { get; set; } = string.Empty;
    public DateTime AtCreation { get; set; }
    public string Status { get; set; } = string.Empty;
}
