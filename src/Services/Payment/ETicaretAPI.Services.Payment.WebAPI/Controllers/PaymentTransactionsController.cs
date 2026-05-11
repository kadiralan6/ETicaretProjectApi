using ETicaretAPI.Services.Payment.Application.Services.PaymentTransactionService;
using ETicaretAPI.Services.Payment.Domain.DTOs.PaymentTransactionDtos;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Services.Payment.WebAPI.Controllers;

[ApiController]
[Route("api/payment/[controller]")]
public class PaymentTransactionsController : ControllerBase
{
    private readonly IPaymentTransactionService _paymentTransactionService;

    public PaymentTransactionsController(IPaymentTransactionService paymentTransactionService)
    {
        _paymentTransactionService = paymentTransactionService;
    }

    [HttpGet("getAllFilter")]
    public async Task<IActionResult> GetAllFilter([FromQuery] GetPaymentTransactionForAdminFilterDto filterDto, CancellationToken cancellationToken = default)
    {
        var result = await _paymentTransactionService.GetPaymentTransactionsFilterAsync(filterDto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("getById/{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var result = await _paymentTransactionService.GetByIdAsync(id, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreatePaymentTransactionDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _paymentTransactionService.CreateAsync(dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdatePaymentTransactionDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _paymentTransactionService.UpdateAsync(dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var result = await _paymentTransactionService.DeleteAsync(id, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
