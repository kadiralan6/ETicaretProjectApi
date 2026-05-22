namespace ETicaretAPI.Services.Catalog.Domain.DTOs.StorefrontDtos;

/// <summary>
/// Marka özet bilgisi — nested olarak product response'a gömülür.
/// </summary>
public class BrandSummaryDto
{
    public string? Name { get; set; }
    public string? Slug { get; set; }
}
