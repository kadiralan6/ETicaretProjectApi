using ETicaretAPI.Common.SharedLibrary.Enums.BasketEnums;

namespace ETicaretAPI.Services.Basket.Application.DTOs;

public class AppliedCampaignDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public CampaignTypeCommonEnum Type { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal? MinimumOrderAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal DiscountAmount { get; set; }
}
