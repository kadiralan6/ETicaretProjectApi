namespace ETicaretAPI.Services.Catalog.Domain.DTOs.StorefrontDtos;

/// <summary>
/// Ana sayfa aggregate response.
/// Frontend tek çağrıda sayfalı öne çıkan ürünler + popüler kategoriler alır.
/// </summary>
public class HomePageDto
{
    public List<ProductCardDto> FeaturedProducts { get; set; } = new();
    public List<CategorySummaryDto> PopularCategories { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
}
