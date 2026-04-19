using ETicaretAPI.Services.Catalog.Application.Services.ProductImageService;
using ETicaretAPI.Common.Application.DTOs.CatalogDtos;
using ETicaretAPI.Services.Catalog.Domain.DTOs.ProductImageDtos;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Services.Catalog.WebAPI.Controllers;

[ApiController]
[Route("api/catalog/[controller]")]
public class ProductImagesController : ControllerBase
{
    private readonly IProductImageService _productImageService;

    public ProductImagesController(IProductImageService productImageService)
    {
        _productImageService = productImageService;
    }

    [HttpGet("getByProductId/{productId:int}")]
    public async Task<IActionResult> GetByProductId(int productId, CancellationToken cancellationToken = default)
    {
        var result = await _productImageService.GetImagesByProductIdAsync(productId, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("getById/{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var result = await _productImageService.GetByIdAsync(id, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromForm] CreateProductImageDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _productImageService.CreateAsync(dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromForm] UpdateProductImageDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _productImageService.UpdateAsync(dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var result = await _productImageService.DeleteAsync(id, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
