using ETicaretAPI.Common.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Domain.DTOs.CartDtos;

public class GetCartDto : BaseDto
{
    public int UserId { get; set; }
    public int? CouponId { get; set; }
    public string? CouponCode { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total { get; set; }
    public List<GetCartItemDto> Items { get; set; } = new();
}
