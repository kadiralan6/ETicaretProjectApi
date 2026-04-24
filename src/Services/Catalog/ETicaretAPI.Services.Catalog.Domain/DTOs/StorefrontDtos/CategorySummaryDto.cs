namespace ETicaretAPI.Services.Catalog.Domain.DTOs.StorefrontDtos;

/// <summary>
/// Kategori özet bilgisi — nested olarak product response'a ve home page'e gömülür.
/// </summary>
public class CategorySummaryDto
{
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public string? ImageUrl { get; set; }
}
