using ETicaretAPI.Services.Payment.Application.DTOs;
using ETicaretAPI.Services.Payment.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Services.Payment.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
  private readonly IOrderService _orderService;

  public OrdersController(IOrderService orderService)
  {
    _orderService = orderService;
  }

  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
  {
    var result = await _orderService.GetByIdAsync(id, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }

  [HttpGet("user/{userId:int}")]
  public async Task<IActionResult> GetByUser(int userId, CancellationToken cancellationToken = default)
  {
    var result = await _orderService.GetByUserIdAsync(userId, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }

  [HttpGet]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
  {
    var result = await _orderService.GetAllAsync(pageNumber, pageSize, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateOrderDto dto, CancellationToken cancellationToken = default)
  {
    var result = await _orderService.CreateAsync(dto, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }

  [HttpPut("status")]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> UpdateStatus([FromBody] UpdateOrderStatusDto dto, CancellationToken cancellationToken = default)
  {
    var result = await _orderService.UpdateStatusAsync(dto, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }

  [HttpPost("{id:int}/cancel")]
  public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken = default)
  {
    var result = await _orderService.CancelAsync(id, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }
}
