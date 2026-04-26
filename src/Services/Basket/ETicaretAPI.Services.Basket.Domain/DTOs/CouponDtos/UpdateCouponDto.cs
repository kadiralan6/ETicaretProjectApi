using ETicaretAPI.Common.SharedLibrary.Enums.BasketEnums;

namespace ETicaretAPI.Services.Basket.Domain.DTOs.CouponDtos;

public class UpdateCouponDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public CampaignTypeCommonEnum Type { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal? MinimumOrderAmount { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsActive { get; set; }
    public int? UsageLimit { get; set; }
}
