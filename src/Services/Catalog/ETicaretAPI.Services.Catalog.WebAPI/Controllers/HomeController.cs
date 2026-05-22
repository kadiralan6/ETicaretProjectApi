using ETicaretAPI.Services.Catalog.Application.Services.StorefrontService;
using ETicaretAPI.Services.Catalog.Domain.DTOs.StorefrontDtos;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Services.Catalog.WebAPI.Controllers;

/// <summary>
/// Ana sayfa endpoint'i — Next.js SSR/SSG frontend için.
/// Tek çağrıda sayfalı featured products + popular categories döner.
/// Mevcut /api/catalog/{everything} Ocelot route'u üzerinden Catalog Service'e ulaşır.
/// </summary>
[ApiController]
[Route("api/catalog/[controller]")]
public class HomeController : ControllerBase
{
    private readonly IStorefrontService _storefrontService;

    public HomeController(IStorefrontService storefrontService)
    {
        _storefrontService = storefrontService;
    }

    /// <summary>
    /// Ana sayfa verisini sayfalı getirir.
    /// Response: sayfalı featuredProducts + popularCategories.
    /// Örnek: GET /api/catalog/home/getHomeData?page=1&amp;pageSize=8&amp;orderBy=CreatedAt
    /// </summary>
    [HttpGet("getHomeData")]
    public async Task<IActionResult> GetHomeData(
        [FromQuery] GetFeaturedProductsFilterDto filterDto,
        CancellationToken cancellationToken = default)
    {
        var result = await _storefrontService.GetHomeDataAsync(filterDto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
