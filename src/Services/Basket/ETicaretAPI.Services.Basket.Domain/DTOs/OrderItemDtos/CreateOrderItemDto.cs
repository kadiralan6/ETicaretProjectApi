namespace ETicaretAPI.Services.Basket.Domain.DTOs.OrderItemDtos;

public class CreateOrderItemDto
{
    public int UserId { get; init; }
    public int ProductId { get; init; }
    public decimal Price { get; init; }
    public decimal? Discount { get; init; }
    public decimal? TotalPrice { get; init; }
    public decimal TotalNetPrice { get; init; }
    public decimal VatAmount { get; init; }
    public int Quantity { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public int? CouponId { get; init; }
}