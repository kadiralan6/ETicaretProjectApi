using ETicaretAPI.Common.Domain.Entities;
using ETicaretAPI.Common.SharedLibrary.Enums.BasketEnums;

public class Coupon : Entity<int>
{
    public string? Code { get; set; }
    public CampaignTypeCommonEnum Type { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal? MinimumOrderAmount { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsActive { get; set; }
    public int? UsageLimit { get; set; }
    public int UsageCount { get; set; }
    public bool IsValid { get; set; }
}