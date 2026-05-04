using ETicaretAPI.Services.Basket.Application.Services.CartItemsService;
using ETicaretAPI.Services.Basket.Domain.DTOs.CartDtos;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Services.Basket.WebAPI.Controllers;

[ApiController]
[Route("api/basket/[controller]")]
public class CartItemsController : ControllerBase
{
    private readonly ICartItemsService _cartItemsService;

    public CartItemsController(ICartItemsService cartItemsService)
    {
        _cartItemsService = cartItemsService;
    }

    [HttpGet("getItemCount/{userId:int}")]
    public async Task<IActionResult> GetItemCount(int userId, CancellationToken cancellationToken = default)
    {
        var result = await _cartItemsService.GetItemCountAsync(userId, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("getByUserId/{userId:int}")]
    public async Task<IActionResult> GetByUserId(int userId, CancellationToken cancellationToken = default)
    {
        var result = await _cartItemsService.GetBasketByUserIdAsync(userId, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("getAllFilter")]
    public async Task<IActionResult> GetAllFilter([FromQuery] GetCartForAdminFilterDto filterDto, CancellationToken cancellationToken = default)
    {
        var result = await _cartItemsService.GetCartItemsFilterAsync(filterDto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("addItem")]
    public async Task<IActionResult> AddItem([FromBody] AddCartItemDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _cartItemsService.AddItemAsync(dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("updateItem")]
    public async Task<IActionResult> UpdateItem([FromBody] UpdateCartItemDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _cartItemsService.UpdateItemAsync(dto.UserId, dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("removeItem/{userId:int}/{cartItemId:int}")]
    public async Task<IActionResult> RemoveItem(int userId, int cartItemId, CancellationToken cancellationToken = default)
    {
        var result = await _cartItemsService.RemoveItemAsync(userId, cartItemId, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("applyCoupon/{userId:int}")]
    public async Task<IActionResult> ApplyCoupon(int userId, [FromBody] ApplyCouponDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _cartItemsService.ApplyCouponAsync(userId, dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("removeCoupon/{userId:int}")]
    public async Task<IActionResult> RemoveCoupon(int userId, CancellationToken cancellationToken = default)
    {
        var result = await _cartItemsService.RemoveCouponAsync(userId, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("clear/{userId:int}")]
    public async Task<IActionResult> Clear(int userId, CancellationToken cancellationToken = default)
    {
        var result = await _cartItemsService.ClearCartAsync(userId, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
