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

    [HttpGet("getItemCount")]
    public async Task<IActionResult> GetItemCount(CancellationToken cancellationToken = default)
    {
        var result = await _cartItemsService.GetItemCountAsync(cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("getByUserId")]
    public async Task<IActionResult> GetByUserId( CancellationToken cancellationToken = default)
    {
        var result = await _cartItemsService.GetBasketByUserIdAsync(cancellationToken);
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
        var result = await _cartItemsService.UpdateItemAsync(dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("removeItem/{cartItemId:int}")]
    public async Task<IActionResult> RemoveItem(int cartItemId, CancellationToken cancellationToken = default)
    {
        var result = await _cartItemsService.RemoveItemAsync(cartItemId, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("applyCoupon")]
    public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _cartItemsService.ApplyCouponAsync(dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("removeCoupon/{cartItemId:int}")]
    public async Task<IActionResult> RemoveCoupon(int cartItemId, CancellationToken cancellationToken = default)
    {
        var result = await _cartItemsService.RemoveCouponAsync(cartItemId, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> Clear(CancellationToken cancellationToken = default)
    {
        var result = await _cartItemsService.ClearCartAsync(cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
