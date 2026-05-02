using ETicaretAPI.Services.Basket.Domain.Enums;

namespace ETicaretAPI.Services.Basket.Domain.DTOs.CartDtos;

public class GetCartForAdminFilterDto : BaseFilterDto<CartItemsOrderByEnum>
{
    public int? UserId { get; set; }
    public int? CouponId { get; set; }
    public int? ProductId { get; set; }
    public string? OrderNumber { get; set; }
}
