using ETicaretAPI.Common.Domain.Entities;

namespace ETicaretAPI.Services.Catalog.Domain.DTOs.ProductDtos;

public class GetProductDto : BaseDto
{
  public int Id { get; set; }
  public string? Code { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }
  public string? Slug { get; set; }
  public decimal Price { get; set; }
  public int StockQuantity { get; set; }
  public bool IsActive { get; set; }
  public bool IsFeatured { get; set; }
  public string? ImageUrl { get; set; }
  public int CategoryId { get; set; }
  public string? CategoryName { get; set; }
  public int BrandId { get; set; }
  public string? BrandName { get; set; }
}
