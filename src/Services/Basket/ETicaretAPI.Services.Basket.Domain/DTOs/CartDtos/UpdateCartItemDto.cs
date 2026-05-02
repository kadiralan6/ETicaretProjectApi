namespace ETicaretAPI.Services.Basket.Domain.DTOs.CartDtos;

public class UpdateCartItemDto
{
    public int CartItemId { get; set; }
    public int Quantity { get; set; }
    public int UserId { get; set; }
    public int? CouponId { get; set; }
    public string? OrderNumber { get; set; }
}
