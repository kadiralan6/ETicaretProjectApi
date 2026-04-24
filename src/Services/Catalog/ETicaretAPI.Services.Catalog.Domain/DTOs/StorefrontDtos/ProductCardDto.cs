namespace ETicaretAPI.Services.Catalog.Domain.DTOs.StorefrontDtos;

/// <summary>
/// Ürün kartı DTO — liste/grid görünümlerinde kullanılır (category products, featured products).
/// Hafif, sadece frontend'in kart renderlemesi için gerekli alanlar.
/// </summary>
public class ProductCardDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "TRY";
    public int StockQuantity { get; set; }
    public bool IsInStock { get; set; }
    public bool IsActive { get; set; }

    // Kapak görseli — liste görünümünde tek görsel yeterli
    public string? CoverImageUrl { get; set; }
    public string? CoverImageAlt { get; set; }

    // Kategori — nested özet
    public string? CategoryName { get; set; }
    public string? CategorySlug { get; set; }

    /// <summary>Canonical URL için: /kategori/elektronik</summary>
    public string? CategoryPath { get; set; }

    // Marka — nested özet
    public string? BrandName { get; set; }
    public string? BrandSlug { get; set; }

    public RatingDto? Rating { get; set; }
}
