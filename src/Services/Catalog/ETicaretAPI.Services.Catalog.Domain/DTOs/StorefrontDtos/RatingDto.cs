namespace ETicaretAPI.Services.Catalog.Domain.DTOs.StorefrontDtos;

/// <summary>
/// Ürün rating bilgisi. Gerçek rating sistemi olmadığı için şimdilik
/// product ID'ye göre deterministik fake değer üretilir.
/// </summary>
public class RatingDto
{
    public double Average { get; set; }
    public int Count { get; set; }
}
