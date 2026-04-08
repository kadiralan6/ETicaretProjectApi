using ETicaretAPI.Services.Catalog.Application.Services.CategoryService;
using ETicaretAPI.Services.Catalog.Domain.DTOs.CategoryDtos;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Services.Catalog.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
  private readonly ICategoryService _categoryService;

  public CategoriesController(ICategoryService categoryService)
  {
    _categoryService = categoryService;
  }

  [HttpGet]
  public async Task<IActionResult> GetAll([FromQuery] GetCategoryForAdminFilterDto filterDto, CancellationToken cancellationToken = default)
  {
    var result = await _categoryService.GetCategoriesFilterAsync(filterDto, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }

  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
  {
    var result = await _categoryService.GetByIdAsync(id, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto, CancellationToken cancellationToken = default)
  {
    var result = await _categoryService.CreateAsync(dto, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }

  [HttpPut]
  public async Task<IActionResult> Update([FromBody] UpdateCategoryDto dto, CancellationToken cancellationToken = default)
  {
    var result = await _categoryService.UpdateAsync(dto, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }

  [HttpDelete("{id:int}")]
  public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
  {
    var result = await _categoryService.DeleteAsync(id, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }
}
