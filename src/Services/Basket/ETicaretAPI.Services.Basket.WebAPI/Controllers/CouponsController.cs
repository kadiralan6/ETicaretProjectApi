using ETicaretAPI.Services.Basket.Application.Services.CouponService;
using ETicaretAPI.Services.Basket.Domain.DTOs.CouponDtos;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Services.Basket.WebAPI.Controllers;

[ApiController]
[Route("api/basket/[controller]")]
public class CouponsController : ControllerBase
{
    private readonly ICouponService _couponService;

    public CouponsController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    [HttpGet("getAllFilter")]
    public async Task<IActionResult> GetAllFilter([FromQuery] GetCouponForAdminFilterDto filterDto, CancellationToken cancellationToken = default)
    {
        var result = await _couponService.GetCouponsFilterAsync(filterDto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("getById/{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var result = await _couponService.GetByIdAsync(id, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateCouponDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _couponService.CreateAsync(dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateCouponDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _couponService.UpdateAsync(dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var result = await _couponService.DeleteAsync(id, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
