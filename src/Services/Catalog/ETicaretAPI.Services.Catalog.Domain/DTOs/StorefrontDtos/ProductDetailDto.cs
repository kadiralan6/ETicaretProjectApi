namespace ETicaretAPI.Services.Catalog.Domain.DTOs.StorefrontDtos;

/// <summary>
/// Ürün detay sayfası için tam response DTO.
/// Frontend tek bir çağrıyla tüm ihtiyacı olan veriyi alır — category, brand, images, breadcrumbs dahil.
/// </summary>
public class ProductDetailDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }

    // SEO meta alanları
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }

    public decimal Price { get; set; }
    public string Currency { get; set; } = "TRY";
    public int StockQuantity { get; set; }
    public bool IsInStock { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public DateTime CreatedAt { get; set; }

    // Nested objeler — frontend ekstra join yapmaz
    public CategorySummaryDto? Category { get; set; }

    /// <summary>
    /// Kategori slug path'i — canonical URL için: /kategori/elektronik
    /// </summary>
    public string? CategoryPath { get; set; }

    public BrandSummaryDto? Brand { get; set; }

    /// <summary>
    /// Ürün detay sayfasına gömülü breadcrumb — ayrı endpoint çağrısına gerek kalmaz.
    /// JSON-LD BreadcrumbList için kullanılır.
    /// </summary>
    public List<BreadcrumbItemDto> Breadcrumbs { get; set; } = new();

    public List<ImageDto> Images { get; set; } = new();

    public RatingDto? Rating { get; set; }
}
