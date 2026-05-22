namespace ETicaretAPI.Services.Payment.Domain.DTOs.PaymentTransactionDtos;

public class CreditCardInfoDto
{
    public string CardNumber { get; set; } = string.Empty;
    public string ExpiryMonth { get; set; } = string.Empty;
    public string ExpiryYear { get; set; } = string.Empty;
    public string Cvv { get; set; } = string.Empty;
    public string CardHolderName { get; set; } = string.Empty;
}
