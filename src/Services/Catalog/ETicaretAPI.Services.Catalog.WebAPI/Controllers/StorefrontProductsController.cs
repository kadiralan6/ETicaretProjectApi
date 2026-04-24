using ETicaretAPI.Services.Catalog.Application.Services.StorefrontService;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Services.Catalog.WebAPI.Controllers;

/// <summary>
/// Storefront ürün endpoint'leri — Next.js SSR/SSG frontend için.
/// Mevcut /api/catalog/{everything} Ocelot route'u üzerinden Catalog Service'e ulaşır.
/// </summary>
[ApiController]
[Route("api/catalog/[controller]")]
public class StorefrontProductsController : ControllerBase
{
    private readonly IStorefrontService _storefrontService;

    public StorefrontProductsController(IStorefrontService storefrontService)
    {
        _storefrontService = storefrontService;
    }

    /// <summary>
    /// Slug ile ürün detayını getirir.
    /// Response: category, brand, images nested olarak dahil.
    /// Kullanım: /urun/iphone-15-pro-max sayfası için.
    /// </summary>
    [HttpGet("GetBySlug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken cancellationToken = default)
    {
        var result = await _storefrontService.GetProductBySlugAsync(slug, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
