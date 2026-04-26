namespace ETicaretAPI.Services.Basket.Domain.DTOs.CartDtos;

public class ApplyCouponDto
{
    public int CartId { get; set; }
    public string CouponCode { get; set; } = string.Empty;
}
