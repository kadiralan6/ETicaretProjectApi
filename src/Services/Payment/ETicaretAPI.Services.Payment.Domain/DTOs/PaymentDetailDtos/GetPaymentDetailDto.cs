using ETicaretAPI.Common.Domain.Entities;

namespace ETicaretAPI.Services.Payment.Domain.DTOs.PaymentDetailDtos;

public class GetPaymentDetailDto : BaseDto
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
}
