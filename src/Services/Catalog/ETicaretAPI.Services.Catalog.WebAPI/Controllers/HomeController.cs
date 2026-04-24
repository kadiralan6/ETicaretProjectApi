using ETicaretAPI.Services.Catalog.Application.Services.StorefrontService;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Services.Catalog.WebAPI.Controllers;

/// <summary>
/// Ana sayfa endpoint'i — Next.js SSR/SSG frontend için.
/// Tek çağrıda featured products + popular categories döner.
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
    /// Ana sayfa verisini getirir.
    /// Response: featuredProducts + popularCategories.
    /// </summary>
    [HttpGet("getHomeData")]
    public async Task<IActionResult> GetHomeData(CancellationToken cancellationToken = default)
    {
        var result = await _storefrontService.GetHomeDataAsync(cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
