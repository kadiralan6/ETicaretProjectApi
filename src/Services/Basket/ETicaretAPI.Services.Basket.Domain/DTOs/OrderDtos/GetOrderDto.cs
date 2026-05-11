using ETicaretAPI.Common.Domain.Entities;
using ETicaretAPI.Services.Basket.Domain.Enums;

namespace ETicaretAPI.Services.Basket.Domain.DTOs.OrderDtos;

public class GetOrderDto : BaseDto
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public decimal TotalPrice { get; set; }
    public int Quantity { get; set; }
    public string? OrderNumber { get; set; }
    public int? CouponId { get; set; }
    public OrderStatusEnum Status { get; set; }
}