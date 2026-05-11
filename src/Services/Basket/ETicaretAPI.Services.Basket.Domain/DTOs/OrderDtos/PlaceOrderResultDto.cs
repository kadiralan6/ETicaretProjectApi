using ETicaretAPI.Services.Basket.Domain.Enums;

namespace ETicaretAPI.Services.Basket.Domain.DTOs.OrderDtos;

public class PlaceOrderResultDto
{
    public int OrderId { get; set; }
    public OrderStatusEnum OrderStatus { get; set; }
    public int PaymentTransactionId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public int PaymentStatus { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
}
