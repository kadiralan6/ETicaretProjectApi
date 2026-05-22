using ETicaretAPI.Common.Domain.Entities;
using ETicaretAPI.Common.SharedLibrary.Enums.BasketEnums;

namespace ETicaretAPI.Services.Basket.Domain.DTOs.CampaignDtos;

public class GetCampaignDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public CampaignTypeCommonEnum Type { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal? MinimumOrderAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public int? UsageLimit { get; set; }
    public int UsageCount { get; set; }
}
