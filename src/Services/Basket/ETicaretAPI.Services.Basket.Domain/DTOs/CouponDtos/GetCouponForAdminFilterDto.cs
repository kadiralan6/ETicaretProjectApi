using ETicaretAPI.Common.SharedLibrary.Enums.BasketEnums;
using ETicaretAPI.Services.Basket.Domain.Enums;

namespace ETicaretAPI.Services.Basket.Domain.DTOs.CouponDtos;

public class GetCouponForAdminFilterDto : BaseFilterDto<CouponOrderByEnum>
{
    public string? Code { get; set; }
    public CampaignTypeCommonEnum? Type { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? ExpiresAfter { get; set; }
}
