namespace ETicaretAPI.Services.Catalog.Domain.DTOs.StorefrontDtos;

/// <summary>
/// Görsel bilgisi — SEO AltText ile birlikte.
/// </summary>
public class ImageDto
{
    public string? Url { get; set; }
    public string? AltText { get; set; }
    public bool IsCover { get; set; }
}
