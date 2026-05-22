using ETicaretAPI.Common.SharedLibrary.Enums.BasketEnums;

namespace ETicaretAPI.Services.Basket.Application.DTOs;

public class AppliedCouponDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public CampaignTypeCommonEnum Type { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal? MinimumOrderAmount { get; set; }
    public DateTime ExpirationDate { get; set; }
    public decimal DiscountAmount { get; set; }
}
