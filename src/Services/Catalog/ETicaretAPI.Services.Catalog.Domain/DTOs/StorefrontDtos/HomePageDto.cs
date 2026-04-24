namespace ETicaretAPI.Services.Catalog.Domain.DTOs.StorefrontDtos;

/// <summary>
/// Ana sayfa aggregate response.
/// Frontend tek çağrıda öne çıkan ürünler + popüler kategoriler alır.
/// </summary>
public class HomePageDto
{
    public List<ProductCardDto> FeaturedProducts { get; set; } = new();
    public List<CategorySummaryDto> PopularCategories { get; set; } = new();
}
