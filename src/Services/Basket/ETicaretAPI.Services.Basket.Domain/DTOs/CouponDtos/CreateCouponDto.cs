using ETicaretAPI.Common.SharedLibrary.Enums.BasketEnums;

namespace ETicaretAPI.Services.Basket.Domain.DTOs.CouponDtos;

public class CreateCouponDto
{
    public string Code { get; init; } = string.Empty;
    public CampaignTypeCommonEnum Type { get; init; }
    public decimal DiscountValue { get; init; }
    public decimal? MinimumOrderAmount { get; init; }
    public DateTime ExpirationDate { get; init; }
    public int? UsageLimit { get; init; }
}
