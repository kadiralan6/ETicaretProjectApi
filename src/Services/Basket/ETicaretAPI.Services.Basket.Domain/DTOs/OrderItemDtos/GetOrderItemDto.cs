using ETicaretAPI.Common.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Domain.DTOs.OrderItemDtos;

public class GetOrderItemDto : BaseDto
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public decimal? Discount { get; set; }
    public decimal? TotalPrice { get; set; }
    public decimal TotalNetPrice { get; set; }
    public decimal VatAmount { get; set; }
    public int Quantity { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int? CouponId { get; set; }
}