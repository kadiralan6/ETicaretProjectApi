using ETicaretAPI.Services.Catalog.Domain.Enums;

namespace ETicaretAPI.Services.Catalog.Domain.DTOs.ProductDtos;

public class GetProductForAdminFilterDto : BaseFilterDto<ProductOrderByEnum>
{
  public string? Name { get; set; }
  public string? Code { get; set; }
  public bool? IsActive { get; set; }
  public bool? IsFeatured { get; set; }
  public int? CategoryId { get; set; }
  public int? BrandId { get; set; }
  public decimal? MinPrice { get; set; }
  public decimal? MaxPrice { get; set; }
}
