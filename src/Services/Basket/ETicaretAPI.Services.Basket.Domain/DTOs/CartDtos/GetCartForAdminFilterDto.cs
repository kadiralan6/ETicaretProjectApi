using ETicaretAPI.Services.Basket.Domain.Enums;

namespace ETicaretAPI.Services.Basket.Domain.DTOs.CartDtos;

public class GetCartForAdminFilterDto : BaseFilterDto<CartOrderByEnum>
{
    public int? UserId { get; set; }
    public int? CouponId { get; set; }
    public decimal? MinTotal { get; set; }
    public decimal? MaxTotal { get; set; }
}
