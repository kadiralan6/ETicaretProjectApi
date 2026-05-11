namespace ETicaretAPI.Services.Basket.Domain.DTOs.OrderDtos;

public class PaymentTransactionResponseDto
{
    public int Id { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public int Status { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
}
