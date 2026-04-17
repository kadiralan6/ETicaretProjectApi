namespace ETicaretAPI.Services.Catalog.Domain.DTOs.ProductDtos;

public class CreateProductDto
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsFeatured { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
}
