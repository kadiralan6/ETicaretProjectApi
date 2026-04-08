namespace ETicaretAPI.Services.Catalog.Domain.DTOs.ProductDtos;

public class UpdateProductDto
{
  public int Id { get; set; }
  public string? Code { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public decimal Price { get; set; }
  public int StockQuantity { get; set; }
  public bool IsActive { get; set; }
  public bool IsFeatured { get; set; }
  public string? ImageUrl { get; set; }
  public int CategoryId { get; set; }
  public int BrandId { get; set; }
}
