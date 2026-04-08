using ETicaretAPI.Services.Catalog.Domain.Enums;

namespace ETicaretAPI.Services.Catalog.Domain.DTOs.BrandDtos;

public class GetBrandForAdminFilterDto : BaseFilterDto<BrandOrderByEnum>
{
  public string? Name { get; set; }
  public bool? IsActive { get; set; }
}
