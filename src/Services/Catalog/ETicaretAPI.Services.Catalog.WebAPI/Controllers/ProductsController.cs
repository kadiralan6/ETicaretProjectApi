using ETicaretAPI.Services.Catalog.Application.Services.ProductService;
using ETicaretAPI.Services.Catalog.Domain.DTOs.ProductDtos;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Services.Catalog.WebAPI.Controllers;

[ApiController]
[Route("api/catalog/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll([FromQuery] GetProductForAdminFilterDto filterDto, CancellationToken cancellationToken = default)
    {
        var result = await _productService.GetProductsFilterAsync(filterDto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("getById/{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var result = await _productService.GetByIdAsync(id, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _productService.CreateAsync(dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _productService.UpdateAsync(dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var result = await _productService.DeleteAsync(id, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("updateProductStock")]
    public async Task<IActionResult> UpdateProductStock([FromBody] UpdateProductQuantityDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _productService.UpdateStockAsync(dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
