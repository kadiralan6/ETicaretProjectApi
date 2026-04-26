using ETicaretAPI.Services.Catalog.Domain.Enums;

namespace ETicaretAPI.Services.Catalog.Domain.DTOs.StorefrontDtos;

public class GetFeaturedProductsFilterDto : BaseFilterDto<FeaturedProductOrderByEnum>
{
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}
