namespace ETicaretAPI.Services.Catalog.Domain.DTOs.StorefrontDtos;

/// <summary>
/// Kategori ürün listesi sayfası için aggregate response.
/// Frontend tek çağrıda kategori bilgisi + sayfalı ürünleri alır.
/// </summary>
public class CategoryProductsResponseDto
{
    public CategorySummaryDto? Category { get; set; }
    public List<ProductCardDto> Products { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
