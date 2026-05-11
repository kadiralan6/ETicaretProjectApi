using ETicaretAPI.Services.Payment.Domain.DTOs.PaymentDetailDtos;
using ETicaretAPI.Services.Payment.Domain.Enums;

namespace ETicaretAPI.Services.Payment.Domain.DTOs.PaymentTransactionDtos;

public class ProcessPaymentDto
{
    public string UserId { get; set; } = string.Empty;
    public int OrderId { get; set; }
    public int AddressId { get; set; }
    public PaymentMethodEnum PaymentMethod { get; set; }
    public CreditCardInfoDto? CardInfo { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";
    public List<PaymentItemDto> Items { get; set; } = [];
}
