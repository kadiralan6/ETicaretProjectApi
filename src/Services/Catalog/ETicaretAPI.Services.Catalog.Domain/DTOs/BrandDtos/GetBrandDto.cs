using ETicaretAPI.Common.Domain.Entities;

namespace ETicaretAPI.Services.Catalog.Domain.DTOs.BrandDtos;

public class GetBrandDto : BaseDto
{
  public int Id { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }
  public string? Slug { get; set; }
  public bool? IsActive { get; set; }
}
