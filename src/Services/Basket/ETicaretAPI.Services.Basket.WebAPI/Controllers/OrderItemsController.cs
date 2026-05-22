using ETicaretAPI.Services.Basket.Application.Services.OrderItemService;
using ETicaretAPI.Services.Basket.Domain.DTOs.OrderItemDtos;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Services.Basket.WebAPI.Controllers;

[ApiController]
[Route("api/basket/[controller]")]
public class OrderItemsController : ControllerBase
{
    private readonly IOrderItemService _orderItemService;

    public OrderItemsController(IOrderItemService orderItemService)
    {
        _orderItemService = orderItemService;
    }

    [HttpGet("getAllFilter")]
    public async Task<IActionResult> GetAllFilter([FromQuery] GetOrderItemForAdminFilterDto filterDto, CancellationToken cancellationToken = default)
    {
        var result = await _orderItemService.GetOrderItemsFilterAsync(filterDto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("getById/{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var result = await _orderItemService.GetByIdAsync(id, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateOrderItemDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _orderItemService.CreateAsync(dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateOrderItemDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _orderItemService.UpdateAsync(dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var result = await _orderItemService.DeleteAsync(id, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}