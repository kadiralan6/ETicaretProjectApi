using ETicaretAPI.Common.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Domain.DTOs.CartDtos;

public class GetCartItemDto : BaseDto
{
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public int Quantity { get; set; }
    public int? CouponId { get; set; }
    public string? OrderNumber { get; set; }
}
