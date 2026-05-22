namespace ETicaretAPI.Services.Catalog.Domain.DTOs.StorefrontDtos;

/// <summary>
/// Breadcrumb öğesi — kategori hiyerarşisinde gezinme için.
/// Kökten yaprak kategoriye sıralı döner.
/// </summary>
public class BreadcrumbItemDto
{
    public string? Name { get; set; }
    public string? Slug { get; set; }
}
