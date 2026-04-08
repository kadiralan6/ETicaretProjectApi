using ETicaretAPI.Services.Payment.Application.DTOs;
using ETicaretAPI.Services.Payment.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Services.Payment.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
  private readonly IPaymentService _paymentService;

  public PaymentsController(IPaymentService paymentService)
  {
    _paymentService = paymentService;
  }

  [HttpPost("process")]
  public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentDto dto, CancellationToken cancellationToken = default)
  {
    var result = await _paymentService.ProcessPaymentAsync(dto, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }

  [HttpPost("order/{orderId:int}/refund")]
  public async Task<IActionResult> Refund(int orderId, CancellationToken cancellationToken = default)
  {
    var result = await _paymentService.RefundAsync(orderId, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }

  [HttpGet("order/{orderId:int}")]
  public async Task<IActionResult> GetByOrder(int orderId, CancellationToken cancellationToken = default)
  {
    var result = await _paymentService.GetPaymentByOrderIdAsync(orderId, cancellationToken);
    return StatusCode(result.StatusCode, result);
  }
}
