using ETicaretAPI.Common.SharedLibrary.Enums.BasketEnums;
using ETicaretAPI.Services.Basket.Domain.Enums;

namespace ETicaretAPI.Services.Basket.Domain.DTOs.CampaignDtos;

public class GetCampaignForAdminFilterDto : BaseFilterDto<CampaignOrderByEnum>
{
    public string? Name { get; set; }
    public CampaignTypeCommonEnum? Type { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? ActiveOn { get; set; }
}
