using ETicaretAPI.Services.Catalog.Application.Services.ProductService;
using ETicaretAPI.Services.Catalog.Domain.DTOs.ProductDtos;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Services.Catalog.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
  private readonly IProductService _productService;

  public ProductsController(IProductService productService)
  {
    _productService = productService;
  }

  [HttpGet]
  public async Task<IActionResult> GetAll([FromQuery] GetProductForAdminFilterDto filterDto, CancellationToken cancellationToken = default)
  {
    var result = await _productService.GetProductsFilterAsync(filterDto, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }

  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
  {
    var result = await _productService.GetByIdAsync(id, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateProductDto dto, CancellationToken cancellationToken = default)
  {
    var result = await _productService.CreateAsync(dto, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }

  [HttpPut]
  public async Task<IActionResult> Update([FromBody] UpdateProductDto dto, CancellationToken cancellationToken = default)
  {
    var result = await _productService.UpdateAsync(dto, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }

  [HttpDelete("{id:int}")]
  public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
  {
    var result = await _productService.DeleteAsync(id, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }

  [HttpPatch("{productId:int}/stock")]
  public async Task<IActionResult> UpdateStock(int productId, [FromQuery] int quantity, CancellationToken cancellationToken = default)
  {
    var result = await _productService.UpdateStockAsync(productId, quantity, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }
}
