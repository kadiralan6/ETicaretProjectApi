namespace ETicaretAPI.Services.Basket.Domain.DTOs.OrderDtos;

public class CreateOrderDto
{
    public int UserId { get; init; }
    public int ProductId { get; init; }
    public decimal Price { get; init; }
    public decimal TotalPrice { get; init; }
    public int Quantity { get; init; }
    public string? OrderNumber { get; init; }
    public int? CouponId { get; init; }
}