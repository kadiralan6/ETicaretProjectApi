namespace ETicaretAPI.Common.Application.DTOs.CatalogDtos;

public class GetCatalogProductDto
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
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? ParentCategoryName { get; set; }
    public int BrandId { get; set; }
    public string? BrandName { get; set; }
    public List<GetProductImageDto> Images { get; set; } = [];
}
