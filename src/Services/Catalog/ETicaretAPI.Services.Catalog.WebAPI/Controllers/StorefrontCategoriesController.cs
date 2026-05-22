using ETicaretAPI.Services.Catalog.Application.Services.StorefrontService;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Services.Catalog.WebAPI.Controllers;

/// <summary>
/// Storefront kategori endpoint'leri — Next.js SSR/SSG frontend için.
/// Mevcut /api/catalog/{everything} Ocelot route'u üzerinden Catalog Service'e ulaşır.
/// </summary>
[ApiController]
[Route("api/catalog/[controller]")]
public class StorefrontCategoriesController : ControllerBase
{
    private readonly IStorefrontService _storefrontService;

    public StorefrontCategoriesController(IStorefrontService storefrontService)
    {
        _storefrontService = storefrontService;
    }

    /// <summary>
    /// Kategoriye ait ürünleri sayfalı getirir.Kategori ürün listeleme sayfası
    /// Kullanım: /kategori/telefon sayfası için.
    /// </summary>
    [HttpGet("GetProductsByCategory/{slug}")]
    public async Task<IActionResult> GetProductsByCategory(
        string slug,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _storefrontService.GetProductsByCategorySlugAsync(slug, page, pageSize, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Kategori breadcrumb hiyerarşisini getirir. Aynı sayfa, JSON-LD için
    /// Kökten yaprağa sıralı — [Elektronik, Telefon, Apple].
    /// Ana Sayfa (/)              → getHomeData
    /// Kategori (/kategori/slug)  → /{slug}/products  +  GetBreadcrumb/{slug}
    /// Ürün Detay (/urun/slug)    → GetBySlug/{slug}
    /// </summary>
    [HttpGet("GetBreadcrumb/{slug}")]
    public async Task<IActionResult> GetBreadcrumb(string slug, CancellationToken cancellationToken = default)
    {
        var result = await _storefrontService.GetBreadcrumbBySlugAsync(slug, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
