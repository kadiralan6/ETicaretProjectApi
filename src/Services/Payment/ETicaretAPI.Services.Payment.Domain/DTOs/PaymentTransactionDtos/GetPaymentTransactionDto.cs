using ETicaretAPI.Common.Domain.Entities;
using ETicaretAPI.Services.Payment.Domain.DTOs.PaymentDetailDtos;
using ETicaretAPI.Services.Payment.Domain.Enums;

namespace ETicaretAPI.Services.Payment.Domain.DTOs.PaymentTransactionDtos;

public class GetPaymentTransactionDto : BaseDto
{
    public int OrderId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public PaymentMethodEnum Method { get; set; }
    public PaymentStatusEnum Status { get; set; }
    public decimal Amount { get; set; }
    public string? FailureReason { get; set; }
    public DateTime? CompletedDate { get; set; }
    public List<GetPaymentDetailDto> Details { get; set; } = [];
}
