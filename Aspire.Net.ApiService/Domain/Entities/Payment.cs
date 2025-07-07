using Aspire.Net.ApiService.Domain.Enums;

namespace Aspire.Net.ApiService.Domain.Entities;

public class Payment
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public TypePayment TypePayment { get; set; }
    public decimal Amount { get; set; }
    public string Details { get; set; }
    public DateTime AtCreation { get; set; }
    public string Status { get; set; }

    public Payment(int productId, TypePayment typePayment, decimal amount, string details)
    {
        ProductId = productId;
        TypePayment = typePayment;
        Amount = amount;
        Details = details;
        AtCreation = DateTime.UtcNow;
        Status = "Pendente";
    }

    protected Payment() { } 
}

