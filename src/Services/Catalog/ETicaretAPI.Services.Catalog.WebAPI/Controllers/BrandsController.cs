using ETicaretAPI.Services.Catalog.Application.Services.BrandService;
using ETicaretAPI.Services.Catalog.Domain.DTOs.BrandDtos;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Services.Catalog.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandsController : ControllerBase
{
  private readonly IBrandService _brandService;

  public BrandsController(IBrandService brandService)
  {
    _brandService = brandService;
  }

  [HttpGet]
  public async Task<IActionResult> GetAll([FromQuery] GetBrandForAdminFilterDto filterDto, CancellationToken cancellationToken = default)
  {
    var result = await _brandService.GetBrandsFilterAsync(filterDto, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }

  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
  {
    var result = await _brandService.GetByIdAsync(id, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateBrandDto dto, CancellationToken cancellationToken = default)
  {
    var result = await _brandService.CreateAsync(dto, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }

  [HttpPut]
  public async Task<IActionResult> Update([FromBody] UpdateBrandDto dto, CancellationToken cancellationToken = default)
  {
    var result = await _brandService.UpdateAsync(dto, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }

  [HttpDelete("{id:int}")]
  public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
  {
    var result = await _brandService.DeleteAsync(id, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }
}

