using ETicaretAPI.Common.Domain.Entities;

namespace ETicaretAPI.Services.Payment.Domain.Entities;

public class PaymentDetail : Entity<int>
{
    public int PaymentTransactionId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int AddressId { get; set; }

    public string CardNumber { get; set; } = string.Empty;
    public string ExpiryMonth { get; set; } = string.Empty;
    public string ExpiryYear { get; set; } = string.Empty;
    public string Cvv { get; set; } = string.Empty;
    public string CardHolderName { get; set; } = string.Empty;

    public PaymentTransaction PaymentTransaction { get; set; } = null!;
}
