namespace ETicaretAPI.Services.Search.Domain.Documents;

/// <summary>
/// Elasticsearch'te indexlenen product belgesi.
/// Catalog servisinden gelen event'ler ile güncellenir.
/// </summary>
public class ProductDocument
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
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int? BrandId { get; set; }
    public string? BrandName { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
