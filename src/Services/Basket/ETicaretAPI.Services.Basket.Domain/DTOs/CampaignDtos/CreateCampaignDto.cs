using ETicaretAPI.Common.SharedLibrary.Enums.BasketEnums;

namespace ETicaretAPI.Services.Basket.Domain.DTOs.CampaignDtos;

public class CreateCampaignDto
{
    public string Name { get; init; } = string.Empty;
    public CampaignTypeCommonEnum Type { get; init; }
    public decimal DiscountValue { get; init; }
    public decimal? MinimumOrderAmount { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public int? UsageLimit { get; init; }
}
