using ETicaretAPI.Services.Payment.Domain.Enums;

namespace ETicaretAPI.Services.Payment.Domain.DTOs.PaymentTransactionDtos;

public class CreatePaymentTransactionDto
{
    public int OrderId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string Currency { get; set; } = "TRY";
    public PaymentMethodEnum Method { get; set; }
    public decimal Amount { get; set; }
}
