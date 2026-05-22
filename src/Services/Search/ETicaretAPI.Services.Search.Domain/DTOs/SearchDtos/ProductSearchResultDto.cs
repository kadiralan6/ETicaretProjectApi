namespace ETicaretAPI.Services.Search.Domain.DTOs.SearchDtos;

public class ProductSearchResultDto
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
    public string? CategoryName { get; set; }
    public string? BrandName { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public double Score { get; set; }
}
