using ETicaretAPI.Services.Basket.Application.Services.CartService;
using ETicaretAPI.Services.Basket.Domain.DTOs.CartDtos;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Services.Basket.WebAPI.Controllers;

[ApiController]
[Route("api/basket/[controller]")]
public class CartsController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartsController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet("getOrCreate/{userId:int}")]
    public async Task<IActionResult> GetOrCreate(int userId, CancellationToken cancellationToken = default)
    {
        var result = await _cartService.GetOrCreateCartAsync(userId, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("getById/{cartId:int}")]
    public async Task<IActionResult> GetById(int cartId, CancellationToken cancellationToken = default)
    {
        var result = await _cartService.GetByIdAsync(cartId, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("getAllFilter")]
    public async Task<IActionResult> GetAllFilter([FromQuery] GetCartForAdminFilterDto filterDto, CancellationToken cancellationToken = default)
    {
        var result = await _cartService.GetCartsFilterAsync(filterDto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("addItem/{userId:int}")]
    public async Task<IActionResult> AddItem(int userId, [FromBody] AddCartItemDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _cartService.AddItemAsync(userId, dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("updateItem/{userId:int}")]
    public async Task<IActionResult> UpdateItem(int userId, [FromBody] UpdateCartItemDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _cartService.UpdateItemAsync(userId, dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("removeItem/{userId:int}/{cartItemId:int}")]
    public async Task<IActionResult> RemoveItem(int userId, int cartItemId, CancellationToken cancellationToken = default)
    {
        var result = await _cartService.RemoveItemAsync(userId, cartItemId, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("applyCoupon/{userId:int}")]
    public async Task<IActionResult> ApplyCoupon(int userId, [FromBody] ApplyCouponDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _cartService.ApplyCouponAsync(userId, dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("removeCoupon/{userId:int}/{cartId:int}")]
    public async Task<IActionResult> RemoveCoupon(int userId, int cartId, CancellationToken cancellationToken = default)
    {
        var result = await _cartService.RemoveCouponAsync(userId, cartId, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("clear/{userId:int}")]
    public async Task<IActionResult> Clear(int userId, CancellationToken cancellationToken = default)
    {
        var result = await _cartService.ClearCartAsync(userId, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
